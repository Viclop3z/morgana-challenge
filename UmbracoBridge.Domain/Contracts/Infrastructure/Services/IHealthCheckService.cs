using UmbracoBridge.Domain.Models;

namespace UmbracoBridge.Domain.Contracts.Infrastructure.Services
{
    public interface IHealthCheckService
    {
        Task<HealthCheckGroup> GetHealthCheck(string token);
    }
}
