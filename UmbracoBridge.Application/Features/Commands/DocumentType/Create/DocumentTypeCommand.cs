using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace UmbracoBridge.Application.Features.Commands.DocumentType.Create
{
    [ExcludeFromCodeCoverage]   
    public class DocumentTypeCommand : IRequest<DocumentTypeResponse>
    {
       
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool AllowedAsRoot { get; set; }
        public string Title { get; set; }
        public bool VariesByCulture { get; set; }
        public bool VariesBySegment { get; set; }
        public object? Collection { get; set; }  
        public bool IsElement { get; set; }
    }
}
