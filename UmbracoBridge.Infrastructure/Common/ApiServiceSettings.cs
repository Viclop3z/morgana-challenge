using System.Diagnostics.CodeAnalysis;

namespace UmbracoBridge.Infrastructure.Common
{
    [ExcludeFromCodeCoverage]
    public class ApiServiceSettings
    {
        public string BaseUrl { get; set; }
        public ApiServiceEndpoint[] Endpoints { get; set; } = [];
        public ApiServiceConfiguration[] Configurations { get; set; }

    }
}
