using System.Diagnostics.CodeAnalysis;

namespace UmbracoBridge.Infrastructure.Common;

[ExcludeFromCodeCoverage]
public class ApiServiceEndpoint
{
    public string Name { get; set; }
    public string Path { get; set; }
}
