using System.Text.Json.Serialization;

namespace UmbracoBridge.Domain.Models
{
    public class HealthCheckGroup
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("items")] 
        public List<HealthCheckGroupItem> Items { get; set; }
    }
}
