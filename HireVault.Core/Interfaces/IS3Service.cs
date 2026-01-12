using System.IO;
using System.Threading.Tasks;

namespace HireVault.Core.Interfaces
{
    public interface IS3Service
    {
        Task UploadFileAsync(Stream fileStream, string fileName, string key, string contentType);
        Task DeleteFileAsync(string key);
    }
}
