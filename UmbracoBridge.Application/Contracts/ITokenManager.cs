namespace UmbracoBridge.Application.Contracts
{
    public interface ITokenManager
    {
        Task<string> GetToken();
    }
}
