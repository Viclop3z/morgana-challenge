using UmbracoBridge.Domain.Models;

namespace UmbracoBridge.Domain.Contracts.Infrastructure.Services
{
    public interface IDocumentTypeService
    {
        Task<DocumentType> CreateDocumentType(string token, DocumentTypeRequest request);
        Task DeleteDocumenttype(string token, string id);
    }
}
