using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmbracoBridge.Application.Contracts;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;

namespace UmbracoBridge.Application.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly ITokenService _tokenService;
        public TokenManager(ITokenService tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        public async Task<string> GetToken()
        {
            var token = await _tokenService.GetToken();

            if (token is null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }    
            return token;
        }
    }
}
