using BeerReporter.AzureQueueLibrary.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureQueueLibrary.QueueConnection
{
    public interface ICloudQueueFactory
    {
        CloudQueueClient GetClient();
    }

    public class CloudQueueFactory : ICloudQueueFactory
    {
        private readonly QueueConfig _config;
        private CloudQueueClient _cloudQueueClient;

        public CloudQueueFactory(QueueConfig queueConfig)
        {
            _config = queueConfig;
        }

        public CloudQueueClient GetClient()
        {
            // If the reusable client's been accessed already, get it
            if (_cloudQueueClient != null)
                return _cloudQueueClient;

            // If not, we get one
            if (!CloudStorageAccount.TryParse(_config.StorageConnectionString, out CloudStorageAccount storageAccount))
                throw new Exception("Could not create storage account with StorageConnectionString configuration");

            _cloudQueueClient = storageAccount.CreateCloudQueueClient();

            return _cloudQueueClient;
        }
    }
}
