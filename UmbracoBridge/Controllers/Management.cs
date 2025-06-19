using MediatR;
using Microsoft.AspNetCore.Mvc;
using UmbracoBridge.Application.Features.Commands.DocumentType.Create;
using UmbracoBridge.Application.Features.Commands.DocumentType.Delete;
using UmbracoBridge.Application.Features.Queries.HealthCheck;

namespace UmbracoBridge.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ManagementController> _logger;

        public ManagementController(IMediator mediator, ILogger<ManagementController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("HealthCheck")]
        public async Task<ActionResult<HealthCheckResponse>> Get()
        {
            return await _mediator.Send(new GetHealthCehckQuery());
        }


        [HttpPost("DocumentType")]
        public async Task<ActionResult<DocumentTypeResponse>> Post([FromBody] DocumentTypeCommand command)
        {
            return await _mediator.Send(command);
        }


        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<Unit>> Delete(string id)
        {
            var command = new DeleteDocumentCommand()
            {
                Id = id
            };
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
 
