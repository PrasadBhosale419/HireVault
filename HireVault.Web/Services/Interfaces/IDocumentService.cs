using Microsoft.AspNetCore.Http;
using HireVault.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HireVault.Web.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<Document> UploadDocumentAsync(IFormFile file, int documentTypeId, string cognitoUserId);
        Task<Document> GetDocumentAsync(int documentId, string cognitoUserId, bool isAdmin = false);
        Task<(string Url, string ContentType)> GetDocumentDownloadUrlAsync(int documentId, string cognitoUserId, bool isAdmin = false);
        Task VerifyDocumentAsync(int documentId, bool isApproved, string notes, string verifiedBy);
        Task<IEnumerable<Document>> GetEmployeeDocumentsAsync(string cognitoUserId);
        Task<IEnumerable<DocumentType>> GetRequiredDocumentTypesAsync();
        Task DeleteDocumentAsync(int documentId, string cognitoUserId);
    }
}
