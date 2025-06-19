namespace UmbracoBridge.Domain.Contracts.Infrastructure.Services
{
    public interface ITokenService
    {
        Task<string> GetToken();
    }
}
