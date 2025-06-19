using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Exceptions;
using UmbracoBridge.Domain.Models;
using UmbracoBridge.Infrastructure.Common;
using UmbracoBridge.Infrastructure.Services.HealthCheck;
using Xunit;

namespace UmbracoBridge.Tests.Services
{
    public class HealthCheckServiceTests
    {
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
        private readonly Mock<IOptionsSnapshot<ApiServiceSettings>> _settingsMock = new();
        private readonly Mock<ILogger<IHealthCheckService>> _loggerMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();

        private readonly ApiServiceSettings _settings = new()
        {
            BaseUrl = "https://fake-api.com",
            Endpoints = [
            
                new() { Name = "HealthCheck", Path = "health-checks/ready" }
            ]
        };

        private HealthCheckService CreateService(HttpMessageHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = new System.Uri("https://fake-api.com")
            };

            _httpClientFactoryMock
                .Setup(f => f.CreateClient("Umbraco"))
                .Returns(client);

            _settingsMock.Setup(s => s.Value).Returns(_settings);

            return new HealthCheckService(
                _httpClientFactoryMock.Object,
                _tokenServiceMock.Object,
                _httpClientFactoryMock.Object,
                _settingsMock.Object,
                _loggerMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task GetHealthCheck_ReturnsHealthCheckGroup_WhenSuccessful()
        {
            // Arrange
            var expected = new HealthCheckGroup
            {
                Items = new List<HealthCheckGroupItem>
                {
                    new HealthCheckGroupItem { Name = "Configuration"},
                    new HealthCheckGroupItem { Name = "Data integrity" },
                    new HealthCheckGroupItem { Name = "Live environment" },
                    new HealthCheckGroupItem { Name = "Permissions" },
                    new HealthCheckGroupItem { Name = "Security" },
                    new HealthCheckGroupItem { Name = "Services" },
                },
            };
            var json = JsonSerializer.Serialize(expected);
            var handler = new StubHttpMessageHandler(json);

            var service = CreateService(handler);

            // Act
            var result = await service.GetHealthCheck("valid-token");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Live environment", result.Items[2].Name);
        }

        [Fact]
        public async Task GetHealthCheck_ThrowsException_WhenUnauthorized()
        {
            // Arrange
            var handler = new StubHttpMessageHandler("Unauthorized", HttpStatusCode.Unauthorized);
            var service = CreateService(handler);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ExternalServiceException>(() => service.GetHealthCheck("bad-token"));
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async Task GetHealthCheck_ThrowsException_WhenTokenIsNull()
        {
            // Arrange
            var handler = new StubHttpMessageHandler("Unauthorized", HttpStatusCode.Unauthorized);
            var service = CreateService(handler);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ExternalServiceException>(() => service.GetHealthCheck(null));
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async Task GetHealthCheck_ThrowsException_WhenDeserializationFails()
        {
            // Arrange
            var invalidJson = "this is not json";
            var handler = new StubHttpMessageHandler(invalidJson);
            var service = CreateService(handler);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => service.GetHealthCheck("token"));
        }
    }

  

}