using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using HireVault.Core.DTOs;
using HireVault.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _configuration;

    public S3Service(IConfiguration configuration)
    {
        _configuration = configuration;

        var accessKey = _configuration["AWS:AccessKey"];
        var secretKey = _configuration["AWS:SecretKey"];
        var sessionToken = _configuration["AWS:SessionToken"];
        var region = _configuration["AWS:Region"];

        var credentials = new SessionAWSCredentials(
            accessKey,
            secretKey,
            sessionToken
        ); 

        _s3 = new AmazonS3Client(
            credentials,
            Amazon.RegionEndpoint.USEast1
        );
    }

    public Task<bool> DeleteFileAsync(string key)
    {
        throw new NotImplementedException();
    }

    public async Task UploadFileAsync(Stream fileStream, string fileName, string key, string contentType)
    {
        var bucketName = _configuration["AWS:BucketName"];
        var buckets = await _s3.ListBucketsAsync();

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType
        };

        await _s3.PutObjectAsync(request);
    }

    Task IS3Service.DeleteFileAsync(string key)
    {
        return DeleteFileAsync(key);
    }

    public async Task<List<ViewDocumentDTO>> GetCandidateDocumentsAsync(int candidateId)
    {
        var bucketName = _configuration["AWS:BucketName"];

        var prefix = $"hirevault/Candidates/{candidateId}/Documents/";

        var request = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = prefix
        };

        var response = await _s3.ListObjectsV2Async(request);

        if (response?.S3Objects == null || !response.S3Objects.Any())
            return new List<ViewDocumentDTO>();

        var documents = new List<ViewDocumentDTO>();

        foreach (var obj in response.S3Objects.Where(x => !x.Key.EndsWith("/")))
        {
            var fileName = Path.GetFileName(obj.Key);

            var presignedUrl = _s3.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = obj.Key,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            documents.Add(new ViewDocumentDTO
            {
                DocumentName = GetDocumentDisplayName(fileName),
                DocumentUrl = presignedUrl,
                ContentType = GetContentType(fileName)
            });
        }

        return documents;
    }


    public async Task<Stream> GetFileAsync(string key)
    {
        var request = new GetObjectRequest
        {
            BucketName = _configuration["AWS:BucketName"],
            Key = key
        };

        var response = await _s3.GetObjectAsync(request);
        return response.ResponseStream;
    }

    private string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLower();

        return ext switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }

    private string GetDocumentDisplayName(string fileName)
    {
        fileName = fileName.ToLower();

        if (fileName.Contains("aadhaar")) return "Aadhaar Card";
        if (fileName.Contains("pan")) return "PAN Card";
        if (fileName.Contains("resume")) return "Resume";
        if (fileName.Contains("resignation")) return "Resignation Letter";
        if (fileName.Contains("salary1")) return "Salary Slip - Month 1";
        if (fileName.Contains("salary2")) return "Salary Slip - Month 2";
        if (fileName.Contains("salary3")) return "Salary Slip - Month 3";

        return Path.GetFileNameWithoutExtension(fileName);
    }

}
