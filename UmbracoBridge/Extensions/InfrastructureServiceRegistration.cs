using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Infrastructure.Common;
using UmbracoBridge.Infrastructure.Services.DocumentTypeService;
using UmbracoBridge.Infrastructure.Services.HealthCheck;
using UmbracoBridge.Infrastructure.Services.Token;


namespace UmbracoBridge.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class InfrastructureServiceRegistration
    {
        
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
                
            services.AddHttpClient();
            var apiSettings = configuration.GetSection("Services").Get<ApiServiceSettings>();
           
            services.AddHttpClient("Umbraco", client =>
            {
                client.BaseAddress = new Uri(apiSettings.BaseUrl);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("HttpClientFactory-Sample");
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSingleton<ITokenService,TokenService>();
            services.AddTransient<IHealthCheckService, HealthCheckService>();  
            services.AddTransient<IDocumentTypeService, DocumentTypeService>();
            return services;
        }
    }
}
