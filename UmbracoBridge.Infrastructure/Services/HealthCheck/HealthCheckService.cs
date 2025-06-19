using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Exceptions;
using UmbracoBridge.Domain.Models;
using UmbracoBridge.Infrastructure.Common;
using Voyager.Anesthesia.Infrastructure.Common;

namespace UmbracoBridge.Infrastructure.Services.HealthCheck
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly ITokenService _tokenService;
        private readonly IOptionsSnapshot<ApiServiceSettings> _settings;  
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IHealthCheckService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _factory;

        public HealthCheckService(IHttpClientFactory clientFactory,ITokenService tokenService,IHttpClientFactory factory, IOptionsSnapshot<ApiServiceSettings> settings, ILogger<IHealthCheckService> logger, IConfiguration configuration)
        {
           _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));   
           _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
           _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
           _settings = settings ?? throw new ArgumentNullException(nameof(settings));
         _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<HealthCheckGroup> GetHealthCheck(string token)
        {
           
            var _servicesSettings = _settings.Value;
            
            var _httpClient = _factory.CreateClient("Umbraco");

            _httpClient.SetBearerToken(token);

            var apiPath = _servicesSettings.GetServiceEndpointPath("HealthCheck");

            var apiResponse = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{apiPath}");

            if (apiResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("Unauthorized access while trying to get health check data. Please check your token.");
                throw new ExternalServiceException("Error getting health check data.", apiResponse.StatusCode);
            }

            string json = await apiResponse.Content.ReadAsStringAsync();
            var healthCheckGroup = JsonSerializer.Deserialize<HealthCheckGroup>(json);
            
            return healthCheckGroup;
        }
         
    }
     
}
