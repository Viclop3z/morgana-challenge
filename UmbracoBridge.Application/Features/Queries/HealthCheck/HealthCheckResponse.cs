namespace UmbracoBridge.Application.Features.Queries.HealthCheck
{
    public class HealthCheckResponse
    {
        public int Total { get; set; }
        public List<HealthCheckGroupItem> Items { get; set; }
    }

}
 