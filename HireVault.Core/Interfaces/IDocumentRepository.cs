using HireVault.Core.Entities;

namespace HireVault.Core.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task<IReadOnlyList<Document>> GetDocumentsByEmployeeIdAsync(int employeeId);
        Task<IReadOnlyList<Document>> GetUnverifiedDocumentsAsync();
        Task<Document?> GetDocumentWithDetailsAsync(int id);
    }
}
