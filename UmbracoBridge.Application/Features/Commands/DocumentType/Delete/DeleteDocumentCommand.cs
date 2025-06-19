using MediatR;

namespace UmbracoBridge.Application.Features.Commands.DocumentType.Delete
{
       
    public class DeleteDocumentCommand : IRequest<Unit>
    {
        public string Id { get; set; }
    }
}
