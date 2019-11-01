using BeerReporter.AzureLibrary.Infrastructure;
using BeerReporter.AzureQueueLibrary.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeerReporter.AzureLibrary.Services.Storage.Blob
{
    public interface ICloudBlobFactory
    {
        Task<CloudBlobContainer> GetBlobContainer();
    }

    public class CloudBlobFactory : ICloudBlobFactory
    {
        private readonly BlobConfig _config;

        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;

        public CloudBlobFactory(BlobConfig config)
        {
            this._config = config;
        }

        public async Task<CloudBlobContainer> GetBlobContainer()
        {
            // If the container's been accessed already, get it
            if (_blobContainer != null)
                return _blobContainer;

            // If not, we create / fetch one
            _blobContainer = GetClient().GetContainerReference(Routes.BlobStorageReports);

            if (await _blobContainer.CreateIfNotExistsAsync())
                await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            return _blobContainer;
        }

        private CloudBlobClient GetClient()
        {
            // If the reusable client's been accessed already, get it
            if (_blobClient != null)
                return _blobClient;

            // If not, we get one
            if (!CloudStorageAccount.TryParse(_config.StorageConnectionString, out CloudStorageAccount storageAccount))
                throw new Exception("Could not create storage account with StorageConnectionString configuration");

            _blobClient = storageAccount.CreateCloudBlobClient();

            return _blobClient;
        }
    }
}
