using System.Diagnostics.CodeAnalysis;
using UmbracoBridge.Infrastructure.Common;

namespace Voyager.Anesthesia.Infrastructure.Common
{
    [ExcludeFromCodeCoverage]
    internal static class ApiServiceOptionsExtensions
    {
        public static string GetServiceEndpointPath(this ApiServiceSettings serviceSettings, string endpointName)
        {
            return serviceSettings.Endpoints.First(x => x.Name == endpointName).Path;
        }
        public static object GetServiceConfiguration(this ApiServiceSettings serviceSettings, string configurationKey)
        {
            return serviceSettings.Configurations.First(x => x.Key == configurationKey).Value;
        }
    }
}
