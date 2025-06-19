using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Exceptions;
using UmbracoBridge.Infrastructure.Common;
using Voyager.Anesthesia.Infrastructure.Common;
using tokenServiceNamespace = UmbracoBridge.Infrastructure.Services.Token;

namespace UmbracoBridge.Infrastructure.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly ApiServiceSettings _ServicesSettings;
        private readonly ILogger<TokenService> _logger;
        private readonly string _baseUrl;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _factory;
        private readonly string _environment;

        
        public TokenService(IOptions<ApiServiceSettings> settings,IHttpClientFactory factory,ILogger<TokenService> logger, IConfiguration configuration)
        {
            _ServicesSettings = settings.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _environment = _configuration.GetSection("Namespace").GetSection("Name").Value;
            _baseUrl = string.Format(_ServicesSettings.BaseUrl, _environment);
            _factory = factory;
        }

        public DateTime ExpiresAt { get; set; }
        public string AccessToken { get; set; }

        public async Task<string> GetToken()
        {
            if (ExpiresAt!=null && DateTime.Now < ExpiresAt)
            {
                return AccessToken;
            }
            var apiPath = _ServicesSettings.GetServiceEndpointPath("Token");
           
            var clientId = _configuration.GetSection("SecuritySettings").GetSection("ClientId").Value;
            var clientSecret = _configuration.GetSection("SecuritySettings").GetSection("ClientSecret").Value;

            if (clientId is null || clientSecret is null)
            {
                _logger.LogInformation("ClientId and ClientSecret should be set for environment {environment}", _environment);
            }
            
            var request = new tokenServiceNamespace.Models.TokenRequest()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
            };
           
            var _httpClient = _factory.CreateClient("Umbraco");
            

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = $"{_baseUrl}/{apiPath}",
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }
            );

            if (tokenResponse.IsError || tokenResponse.AccessToken is null)
            {
                _logger.LogError($"Error obtaining a token: {tokenResponse.ErrorDescription}");

                throw new UnauthorizedAccessException("Error obtaining a token." );

                return null;
            }
            
            ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            AccessToken = tokenResponse.AccessToken;

            return tokenResponse.AccessToken;

        }
    }
}
  