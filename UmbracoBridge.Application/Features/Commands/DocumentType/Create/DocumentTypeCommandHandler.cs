using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using UmbracoBridge.Application.Constants;
using UmbracoBridge.Application.Contracts;
using UmbracoBridge.Domain.Contracts.Infrastructure.Services;
using UmbracoBridge.Domain.Exceptions;
using UmbracoBridge.Domain.Models;

namespace UmbracoBridge.Application.Features.Commands.DocumentType.Create
{
    public class DocumentTypeCommandHandler : IRequestHandler<DocumentTypeCommand, DocumentTypeResponse>
    {
        private readonly IDocumentTypeService _documentTypeService;
        private readonly ITokenManager _tokenManagerService;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentTypeCommandHandler> _logger;   

        public DocumentTypeCommandHandler(IDocumentTypeService documentTypeService,ITokenManager tokenManagerService,IMapper mapper,ILogger<DocumentTypeCommandHandler> logger) 
        {
            _documentTypeService = documentTypeService ?? throw new ArgumentNullException(nameof(documentTypeService));    
            _tokenManagerService = tokenManagerService ?? throw new ArgumentNullException(nameof(tokenManagerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<DocumentTypeResponse> Handle(DocumentTypeCommand command, CancellationToken cancellationToken)
        {
            var token = await _tokenManagerService.GetToken();

            var request = _mapper.Map<DocumentTypeRequest>(command);

            var response = await _documentTypeService.CreateDocumentType(token, request);

            if (response is null)
            {
                _logger.LogError(ApplicationErrorMessagesConstants.DocumentTypeCreationFailed);

                throw new ApplicationException(ApplicationErrorMessagesConstants.DocumentTypeCreationFailed);
            }
            var result = _mapper.Map<DocumentTypeResponse>(response!);

            return result;
        }
    }
}
    
