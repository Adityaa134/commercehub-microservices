using Azure.Storage.Blobs;
using ProductsService.Core.ServiceContracts;

namespace ProductsService.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public ImageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task DeleteImageAsync(string blobName, string containerName)
        {
            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentNullException(nameof(blobName), "Blob name cannot be null or empty.");
            if(string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException(nameof(containerName), "Container name cannot be null or empty.");

            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

        public string GetImageUrl(string containerName, string blobName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
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

            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Name;
        }
    }
}
