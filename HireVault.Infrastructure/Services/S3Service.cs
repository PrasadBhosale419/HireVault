using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
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
}
