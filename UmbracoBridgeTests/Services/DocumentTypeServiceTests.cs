using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Text;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Exceptions;
using UmbracoBridge.Domain.Models;
using UmbracoBridge.Infrastructure.Common;
using UmbracoBridge.Infrastructure.Services.DocumentTypeService;
using Xunit;

public class DocumentTypeServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IOptionsSnapshot<ApiServiceSettings>> _settingsMock = new();
    private readonly Mock<ILogger<IHealthCheckService>> _loggerMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IConfigurationSection> _namespaceSectionMock = new();
    private readonly Mock<IConfigurationSection> _nameSectionMock = new();

    private readonly DocumentTypeService _service;

    public DocumentTypeServiceTests()
    {
        
        _namespaceSectionMock.Setup(x => x.GetSection("Name")).Returns(_nameSectionMock.Object);
        _nameSectionMock.Setup(x => x.Value).Returns("dev");
        _configurationMock.Setup(x => x.GetSection("Namespace")).Returns(_namespaceSectionMock.Object);

        var settings = new ApiServiceSettings
        {
            Endpoints = [
               new ApiServiceEndpoint  { Name = "DocumentType",Path= "-create-doc" },
               new ApiServiceEndpoint  {Name= "DeleteDocumentType",Path= "delete-doc/{0}" }
            ]
        };

        _settingsMock.Setup(s => s.Value).Returns(settings);

        _service = new DocumentTypeService(
            _httpClientFactoryMock.Object,
            _tokenServiceMock.Object,
            _settingsMock.Object,
            _loggerMock.Object,
            _configurationMock.Object
        );
    }

     
    [Fact]
    public async Task CreateDocumentType_ValidRequest_ReturnsDocumentType()
    {
        // Arrange
        var token = "valid-token";
        var request = new DocumentTypeRequest();
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Headers = { Location = new Uri("https://fake/api/document-types/123") },
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        });

        var client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = new Uri("https://fake")
        };

        _httpClientFactoryMock.Setup(f => f.CreateClient("Umbraco")).Returns(client);

        // Act
        var result = await _service.CreateDocumentType(token, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123", result.Id);
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    [Fact]
    public async Task CreateDocumentType_ApiReturnsError_ThrowsExternalServiceException()
    {
        // Arrange
        var token = "valid-token";
        var request = new DocumentTypeRequest();

        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        });

        var client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = new Uri("https://fake")
        };

        _httpClientFactoryMock.Setup(f => f.CreateClient("Umbraco")).Returns(client);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ExternalServiceException>(() =>
            _service.CreateDocumentType(token, request));

        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteDocumentType_TokenIsNull_ThrowsUnauthorizedAccessException()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.DeleteDocumenttype(null!, "doc-id"));

        Assert.Contains("Token cannot be null", ex.Message);
    }


    [Fact]
    public async Task DeleteDocumentType_ApiReturnsError_ThrowsExternalServiceException()
    {
        // Arrange
        var token = "valid-token";
        var id = "doc-id";

        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        });

        var client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = new Uri("https://fake")
        };

        _httpClientFactoryMock.Setup(f => f.CreateClient("Umbraco")).Returns(client);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ExternalServiceException>(() =>
            _service.DeleteDocumenttype(token, id));

        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
    }


    [Fact]
    public void GetId_NullLocation_ReturnsEmptyString()
    {
        var result = _service.GetId(null!);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetId_ValidLocation_ReturnsId()
    {
        var result = _service.GetId("https://api/something/789?param=value");
        Assert.Equal("789", result);
    }
}