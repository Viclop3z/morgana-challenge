using MediatR;
using System.Data;
using UmbracoBridge.Application.Features.Commands.DocumentType.Create;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Models;

namespace UmbracoBridge.Application.Features.Commands.DocumentType.Delete
{
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
    {
        private readonly ITokenService _tokenService;
        private readonly IDocumentTypeService _documentTypeService;

        public DeleteDocumentCommandHandler(ITokenService tokenService, IDocumentTypeService documentTypeService) 
        { 
           _documentTypeService = documentTypeService ?? throw new ArgumentNullException(nameof(documentTypeService));  
           _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));  
        }
        public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetToken();
        
             await _documentTypeService.DeleteDocumenttype(token, request.Id.ToString());

            return Unit.Value;
        }
    }
}
