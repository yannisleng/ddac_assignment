using Amazon.S3;
using Amazon.S3.Model;
using ddat_assignment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ddat_assignment.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        public FilesController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync([FromForm] IFormFile file, [FromForm] string bucketName, [FromForm] string? prefix)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName + ".jpg" : $"{prefix?.TrimEnd('/')}/{file.FileName + ".jpg"}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", ".jpg");
            await _s3Client.PutObjectAsync(request);
            return Ok($"File {prefix}/{file.FileName + ".jpg"} uploaded to S3 successfully!");
        }

        [HttpGet("get-by-key")]
        public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");

            // Generate a presigned URL for the S3 object
            var presignedUrl = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(10) 
            });

            var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
            var deliveryDate = s3Object.LastModified; 

            return Ok(new
            {
                presignedUrl,
                deliveryDate
            });
        }
    }
}
