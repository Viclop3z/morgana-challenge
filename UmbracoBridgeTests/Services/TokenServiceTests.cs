using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using UmbracoBridge.Infrastructure.Common;
using UmbracoBridge.Infrastructure.Services.Token;
using Xunit;

namespace UmbracoBridge.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
        private readonly Mock<ILogger<TokenService>> _loggerMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();
        private readonly Mock<IOptions<ApiServiceSettings>> _settingsMock = new();

        private readonly ApiServiceSettings _serviceSettings = new()
        {
            BaseUrl = "https://fake-api.com/{0}",
            Endpoints =
            [
              new ApiServiceEndpoint{ Name = "Token",Path ="token"},
              new ApiServiceEndpoint { Name = "HealthCheck", Path = "healthcheck" }
            ]
        };


        private TokenService CreateService(HttpMessageHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = new System.Uri("https://fake-api.com/dev")
            };

            _httpClientFactoryMock
                .Setup(f => f.CreateClient("Umbraco"))
                .Returns(client);

            _configurationMock
                .Setup(c => c.GetSection("Namespace").GetSection("Name").Value)
                .Returns("dev");

            _configurationMock
                .Setup(c => c.GetSection("SecuritySettings").GetSection("ClientId").Value)
                .Returns("client-id");

            _configurationMock
                .Setup(c => c.GetSection("SecuritySettings").GetSection("ClientSecret").Value)
                .Returns("client-secret");

            _settingsMock.Setup(s => s.Value).Returns(_serviceSettings);

            return new TokenService(_settingsMock.Object, _httpClientFactoryMock.Object, _loggerMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task GetToken_ReturnsAccessToken_WhenValidResponse()
        {
            // Arrange
            var handler = new StubHttpMessageHandler("{\"access_token\":\"my-token\",\"expires_in\":3600}");
            var service = CreateService(handler);

            // Act
            var token = await service.GetToken();

            // Assert
            Assert.Equal("my-token", token);
            Assert.False(string.IsNullOrEmpty(service.AccessToken));
            Assert.True(service.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task GetToken_ReturnsCachedToken_WhenNotExpired()
        {
            // Arrange
            var handler = new StubHttpMessageHandler("{\"access_token\":\"cached-token\",\"expires_in\":3600}");
            var service = CreateService(handler);

            // Get token once
            var token1 = await service.GetToken();

            // Act (force re-fetch, but should return cached)
            var token2 = await service.GetToken();

            // Assert
            Assert.Equal("cached-token", token2);
            _httpClientFactoryMock.Verify(f => f.CreateClient("Umbraco"), Times.Once);
        }

        [Fact]
        public async Task GetToken_ThrowsException_WhenAccessTokenMissing()
        {
            // Arrange
            var handler = new StubHttpMessageHandler("{\"error\":\"invalid_client\"}", HttpStatusCode.BadRequest);
            var service = CreateService(handler);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetToken());
            Assert.Equal("Error obtaining a token.", ex.Message);
        }

      
    }


    public class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;
        private readonly HttpStatusCode _statusCode;

        public StubHttpMessageHandler(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responseContent = content;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_responseContent)
            });
        }
    }
    
}