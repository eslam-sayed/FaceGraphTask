using Microsoft.Azure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FaceGraph.API.Common;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;

namespace FaceGraph.API.Services
{
    public class ImagesService : IImagesService
    {
        private readonly ILogger<ImagesService> _logger;
        private CloudBlobClient _cloudBlobClient;
        private const string ContainerName = "images";

        public ImagesService(string connectionString, ILogger<ImagesService> logger)
        {
            CloudStorageAccount storageAccount;
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is null or empty!", nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            try
            {
                // Check whether the connection string can be parsed.
                if (!CloudStorageAccount.TryParse(connectionString, out storageAccount))
                {
                    throw new Exception(
                        "A connection string has not been defined in the system environment variables. " +
                        "Add an environment variable named 'CONNECT_STR' with your storage " +
                        "connection string as a value.");
                }

                //instaiate the service client 
                InitateBlobClient(storageAccount);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot create Blob client by connection {connectionString}", connectionString);
                throw;
            }
        }

        public async Task<IEnumerable<IListBlobItem>> GetImages()
        {
            var images = new List<IListBlobItem>();
            BlobContinuationToken blobContinuationToken = null;
            try
            {
                var cloudBlobContainer = _cloudBlobClient.GetContainerReference(ContainerName);
                //await cloudBlobContainer.CreateIfNotExistsAsync();
                do
                {
                    BlobResultSegment results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    // Get the value of the continuation token returned by the listing call.
                    blobContinuationToken = results.ContinuationToken;
                    images.AddRange(results.Results);
                }
                // Loop while the continuation token is not null.
                while (blobContinuationToken != null);

                return images;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot get images that are in container {ContainerName}", ContainerName);
                throw;
            }

        }

        public async Task<bool> UploadImage(Stream fileStream, string fileName)
        {
            try
            {
                // Get reference to the blob container
                CloudBlobContainer cloudBlobContainer = _cloudBlobClient.GetContainerReference(ContainerName);

                // Get the reference to the block blob from the container
                CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                
                // Upload the file
                await blockBlob.UploadFromStreamAsync(fileStream);

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot upload the image {fileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteImages()
        {
            try
            {
                // Get reference to the blob container
                CloudBlobContainer cloudBlobContainer = _cloudBlobClient.GetContainerReference(ContainerName);

                if (cloudBlobContainer != null)
                {
                    foreach (var blob in cloudBlobContainer.ListBlobs())
                    {
                        if (blob.GetType() == typeof(CloudBlockBlob))
                        {
                            await ((CloudBlockBlob)blob).DeleteIfExistsAsync();
                        }
                    }
                }

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot clear the container {ContainerName}", ContainerName);
                throw;
            }
        }

        public async Task<bool> DeleteImage(string imageName)
        {
            try
            {
                // Get reference to the blob container
                CloudBlobContainer cloudBlobContainer = _cloudBlobClient.GetContainerReference(ContainerName);

                if (cloudBlobContainer == null)
                    return await Task.FromResult(false);

                var blob = cloudBlobContainer.GetBlockBlobReference(imageName);
                await blob.DeleteIfExistsAsync();

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot delete image {imageName}", imageName);
                throw;
            }
        }

        private void InitateBlobClient(CloudStorageAccount storageAccount)
        {
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

    }

}
