using FluentValidation;
using UmbracoBridge.Application.Features.Queries.HealthCheck;

namespace Voyager.AnesthesiaManagement.Application.Feature.Procedures.Queries.GetProcedure
{
    public class GetHealthCheckQueryValidator : AbstractValidator<GetHealthCehckQuery>
    {
        public GetHealthCheckQueryValidator()
        { 
        }
    }
}