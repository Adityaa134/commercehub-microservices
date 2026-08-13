namespace ProductsService.Core.ServiceContracts
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Stream stream, string fileName, string containerName);
        Task DeleteImageAsync(string blobName, string containerName);
        Task<string> ReplaceImageAsync(Stream stream,string newFileName,string oldBlobName,string containerName);
        string GetImageUrl(string containerName, string blobName);
    }
}
