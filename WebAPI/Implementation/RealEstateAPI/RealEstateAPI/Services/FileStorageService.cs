using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3.Transfer;
using Amazon.S3;
using RealEstateAPI.Connector;
using static System.Net.Mime.MediaTypeNames;
using Amazon.S3.Model;

namespace RealEstateAPI.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly AWSConnector _awsConnector;
        private readonly string _tableName = "Property";
        private readonly Table _table;
        private readonly String bucketName = "assignment4realestate";

        public FileStorageService(AWSConnector awsConnector)
        {
            _awsConnector = awsConnector;
            _table = _awsConnector.LoadContentTable(_tableName);
        }

        public async Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> propertyImageUrls)
        {
            var imageUrls = new List<string>();
            foreach (var image in propertyImageUrls)
            {
                if (image.Length > 0)
                {
                    var imageUrl = await UploadFileAsync(image);
                    imageUrls.Add(imageUrl);

                }
            }
            return imageUrls;
        }

        public async Task<string> SaveSingleImageAsync(IFormFile propertyImageUrl)
        {
            if (propertyImageUrl == null || propertyImageUrl.Length == 0)
            {
                return null;
            }

            var imageUrl = await UploadFileAsync(propertyImageUrl);
            return imageUrl;
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            try
            {
                await DeleteFileFromS3Async(imageUrl);
            }
            catch (AmazonS3Exception s3Ex)
            {
                Console.WriteLine($"Error occurred while deleting image from S3: {s3Ex.Message}");

                // Optionally, you can also log the stack trace for more detailed debugging information
                Console.WriteLine(s3Ex.StackTrace);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {

            var fileName = Path.GetFileName(file.FileName);
            var key = fileName;
            //+ Path.GetExtension(file.FileName);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = key,
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead
            };

            var fileTransferUtility = new TransferUtility(_awsConnector.S3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);
            return $"https://{bucketName}.s3.amazonaws.com/{key}";
        }

        public async Task DeleteFileFromS3Async(string fileUrl)
        {
            var key = Path.GetFileName(new Uri(fileUrl).AbsolutePath);
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            await _awsConnector.S3Client.DeleteObjectAsync(deleteObjectRequest);
        }

        
    }
}
