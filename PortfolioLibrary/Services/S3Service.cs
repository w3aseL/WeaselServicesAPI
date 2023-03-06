using Amazon.S3;
using Amazon.S3.Model;
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
            using var client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey);

            var objReq = new PutObjectRequest()
            {
                BucketName = _settings.Bucket,
                FilePath = $"{ folder }/{ file.FileName }",
                InputStream = file.OpenReadStream()
            };

            await client.PutObjectAsync(objReq);
        }

        public async Task DeleteFile(string fileKey)
        {
            using var client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey);

            var objReq = new DeleteObjectRequest()
            {
                BucketName = _settings.Bucket,
                Key = fileKey
            };

            await client.DeleteObjectAsync(objReq);
        }

        public string GetSiteUrlFromBucketName()
        {
            return $"https://{ _settings.Bucket }/";
        }
    }
}
