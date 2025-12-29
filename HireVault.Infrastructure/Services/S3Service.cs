using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using HireVault.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HireVault.Infrastructure.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"] ?? throw new ArgumentNullException("AWS BucketName is not configured");
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string key, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType,
                CannedACL = S3CannedACL.Private
            };

            var response = await _s3Client.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return key;
            }

            throw new Exception("Failed to upload file to S3");
        }

        public async Task<bool> DeleteFileAsync(string key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.DeleteObjectAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}
