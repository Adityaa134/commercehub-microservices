using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using UsersService.Core.ServiceContracts;

namespace UsersService.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<ImageService> _logger;

        public ImageService(BlobServiceClient blobServiceClient,ILogger<ImageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public string GetImageSasUrl(string containerName, string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentNullException(nameof(blobName), "Blob name cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentNullException(nameof(containerName), "Container name cannot be null or empty.");

            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            if (!blobClient.CanGenerateSasUri)
            {
                _logger.LogError("Sas URI is not generated. Ensure the Azure Storage connection string contains a valid AccountKey.");
                throw new InvalidOperationException("Cannot generate SAS URI.");
            }

            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.Add(TimeSpan.FromHours(24))
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        public async Task DeleteImageAsync(string blobName, string containerName)
        {
            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentNullException(nameof(blobName), "Blob name cannot be null or empty.");
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName), "Container name cannot be null or empty.");

            try
            {
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete blob {BlobName} from container {ContainerName}", blobName, containerName);
            }
        }

        public async Task<string> ReplaceImageAsync(Stream stream, string newFileName, string oldBlobName, string containerName)
        {
            if (string.IsNullOrEmpty(newFileName))
                throw new ArgumentNullException(nameof(newFileName), "File name cannot be null or empty.");
            if (string.IsNullOrEmpty(oldBlobName))
                throw new ArgumentNullException(nameof(oldBlobName), "Blob name cannot be null or empty.");
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName), "Container name cannot be null or empty.");

            string blobName = await UploadImageAsync(stream, newFileName, containerName);
            await DeleteImageAsync(oldBlobName, containerName);
            return blobName;
        }

        public async Task<string> UploadImageAsync(Stream stream, string fileName, string containerName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty.");
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName), "Container name cannot be null or empty.");

            try 
            { 
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(stream, overwrite: true);
                return blobClient.Name;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload blob {FileName} to container {ContainerName}", fileName, containerName);
                throw new InvalidOperationException($"Failed to upload image '{fileName}'");
            }
        }
    }
}
