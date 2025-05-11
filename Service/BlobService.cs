using Azure.Storage.Blobs;

namespace Venue_Booking_System.Service
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        //go to storage browser
        private readonly string _containerName = "blobtest";

        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }
        public async Task<string> uploadAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
            return blobClient.Uri.ToString();
        }
        

        public async Task<bool> BlobExistsAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            return await blobClient.ExistsAsync();
        }

        //method to delete the blob
        public async Task DeleteBlobAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

    }
}
