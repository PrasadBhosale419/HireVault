using HireVault.Core.Entities;

namespace HireVault.Core.Interfaces
{
    public interface IDocumentTypeRepository : IRepository<DocumentType>
    {
        Task<IReadOnlyList<DocumentType>> GetRequiredDocumentTypesAsync();
    }
}
