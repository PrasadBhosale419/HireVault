namespace HireVault.Core.Interfaces
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderPath);
        Task<Stream> GetFileAsync(string fileKey);
        Task<string> GetFileUrlAsync(string fileKey, TimeSpan? expiryTime = null);
        Task DeleteFileAsync(string fileKey);
        Task<bool> FileExistsAsync(string fileKey);
        string GeneratePreSignedUrl(string fileKey, TimeSpan expiryTime);
    }
}
