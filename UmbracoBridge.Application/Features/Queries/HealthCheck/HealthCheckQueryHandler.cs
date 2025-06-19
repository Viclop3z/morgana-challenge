using MediatR;
using System.Net.Http.Json;
using IdentityModel.Client;
using System.Text.Json;
using System.ComponentModel;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Application.Contracts;
using System.Net;


namespace UmbracoBridge.Application.Features.Queries.HealthCheck
{
    public record GetHealthCehckQuery() : IRequest<HealthCheckResponse>;
    public class HealthCheckQueryHandler : IRequestHandler<GetHealthCehckQuery, HealthCheckResponse>
    {
        private readonly ITokenManager _tokenManagerService;
        private readonly IHealthCheckService _healthCheckService;
        public HealthCheckQueryHandler(ITokenManager tokenManagerService,IHealthCheckService healthCheckService)
        {
            _tokenManagerService = tokenManagerService ?? throw new ArgumentNullException(nameof(tokenManagerService));
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
        }

        public async Task<HealthCheckResponse> Handle(GetHealthCehckQuery request, CancellationToken cancellationToken)
        {
            var token = await _tokenManagerService.GetToken();
             
            var healthCheck = await _healthCheckService.GetHealthCheck(token);

            if (healthCheck is null)
            {
                return null;
            }

            return new HealthCheckResponse
            {
                Total = healthCheck.Total,
                Items = healthCheck.Items.Select(item => new HealthCheckGroupItem
                {
                    Name = item.Name,

                }).ToList(),
            };

        }
    }
}
