using System.Text.Json.Serialization;

namespace UmbracoBridge.Domain.Models
{
    public class HealthCheckGroupItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
