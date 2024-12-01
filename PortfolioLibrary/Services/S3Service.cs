using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Azure;
using Microsoft.AspNetCore.Http;

namespace PortfolioLibrary
{
    public class S3Service
    {
        private readonly S3Settings _settings;

        public S3Service(S3Settings settings)
        {
            _settings = settings;
        }

        public async Task UploadFile(string folder, IFormFile file)
        {
            using var client = new AmazonS3Client(GetCredentials(), RegionEndpoint.USEast2);

            var stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;

            PutObjectRequest request = new PutObjectRequest();
            request.InputStream = stream;
            request.BucketName = _settings.Bucket;
            request.Key = $"{GetTestModeKey()}{folder}/{file.FileName}";
            await client.PutObjectAsync(request);
        }

        public async Task DeleteFile(string fileKey)
        {
            using var client = new AmazonS3Client(GetCredentials(), RegionEndpoint.USEast2);

            var objReq = new DeleteObjectRequest()
            {
                BucketName = _settings.Bucket,
                Key = $"{GetTestModeKey()}{fileKey}"
            };

            await client.DeleteObjectAsync(objReq);
        }

        public string GetSiteUrlFromBucketName()
        {
            return $"https://{ _settings.Bucket }/{GetTestModeKey()}";
        }

        private string GetTestModeKey()
        {
            return $"{(_settings.TestMode ? "test-wsl-srv/" : "")}";
        }

        private AWSCredentials GetCredentials() => new BasicAWSCredentials(_settings.AccessKey, _settings.SecretKey);
    }
}
