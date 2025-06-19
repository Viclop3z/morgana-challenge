using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Exceptions;
using UmbracoBridge.Domain.Models;
using UmbracoBridge.Infrastructure.Common;
using UmbracoBridge.Infrastructure.Constants;
using Voyager.Anesthesia.Infrastructure.Common;

namespace UmbracoBridge.Infrastructure.Services.DocumentTypeService
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly ITokenService _tokenService;
        private readonly IOptionsSnapshot<ApiServiceSettings> _settings;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IHealthCheckService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _factory;   

        public DocumentTypeService(IHttpClientFactory clientFactory, ITokenService tokenService, IOptionsSnapshot<ApiServiceSettings> settings, ILogger<IHealthCheckService> logger, IConfiguration configuration)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _factory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<DocumentType> CreateDocumentType(string token, DocumentTypeRequest request    )
        {
           
            var _servicesSettings = _settings.Value;
            var environment = _configuration.GetSection("Namespace").GetSection("Name").Value; 
            var _httpClient = _factory.CreateClient("Umbraco");

            _httpClient.SetBearerToken(token);
 
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


            var apiPath = _servicesSettings.GetServiceEndpointPath("DocumentType");
            var apiResponse = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/{apiPath}",content);

            if (apiResponse.StatusCode != System.Net.HttpStatusCode.Created)
            {
                _logger.LogError("Error creating document type");

                throw new ExternalServiceException("Error creating document type.", apiResponse.StatusCode);
            }
            string json = await apiResponse.Content.ReadAsStringAsync();
            
            var resourceId = GetId(apiResponse?.Headers?.Location?.ToString()!);

            return new DocumentType
            { 
                Id = resourceId 
            };
        }

        public async Task DeleteDocumenttype(string token, string id)
        {
            if (token is null)
            {
                throw new UnauthorizedAccessException("Token cannot be null. Please ensure you have a valid token before making this request.");
            }

            var _servicesSettings = _settings.Value;
            var environment = _configuration.GetSection("Namespace").GetSection("Name").Value;
            var _httpClient = _factory.CreateClient("Umbraco");
            _httpClient.SetBearerToken(token);
            var apiPath = _servicesSettings.GetServiceEndpointPath("DeleteDocumentType");
            apiPath = string.Format(apiPath, id);

            var apiResponse = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{apiPath}");
            if (apiResponse.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(string.Format(InfrastructureErrorMessagesConstants.ErrorDeletingDocument,id));
                throw new ExternalServiceException(string.Format(InfrastructureErrorMessagesConstants.ErrorDeletingDocument, id), apiResponse.StatusCode);
            }
        }

        public string GetId(string location)
        {
            if (location != null)
            {
                return location.Split('/').LastOrDefault()?.Split('?').FirstOrDefault() ?? string.Empty;
            }
            return string.Empty;
        }

    }
}
