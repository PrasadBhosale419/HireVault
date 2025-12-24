using System.Collections.Generic;
using System.Threading.Tasks;
using HireVault.Core.Entities;
namespace HireVault.Core.Interfaces
{
    public interface IDocumentService
    {
        Task<Document> GetDocumentByIdAsync(int id);
        Task<IEnumerable<Document>> GetDocumentsByEmployeeIdAsync(int employeeId);
        Task<Document> UploadDocumentAsync(Document document);
        Task<bool> DeleteDocumentAsync(int id);
        Task<bool> VerifyDocumentAsync(int id, bool isVerified);
    }
}