using Amazon.S3.Model;
using HireVault.Core.DTOs;
using System.IO;
using System.Threading.Tasks;

namespace HireVault.Core.Interfaces
{
    public interface IS3Service
    {
        Task UploadFileAsync(Stream fileStream, string fileName, string key, string contentType);
        Task DeleteFileAsync(string key);
        Task<List<ViewDocumentDTO>> GetCandidateDocumentsAsync(int candidateId);
        Task<Stream> GetFileAsync(string key);
    }
}
