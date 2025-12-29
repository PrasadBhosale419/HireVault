using System.IO;
using System.Threading.Tasks;

namespace HireVault.Core.Interfaces
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string key, string contentType);
        Task<bool> DeleteFileAsync(string key);
    }
}
