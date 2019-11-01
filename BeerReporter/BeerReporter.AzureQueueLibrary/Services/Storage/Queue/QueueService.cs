using BeerReporter.AzureQueueLibrary.Messages;
using BeerReporter.AzureQueueLibrary.MessageSerializer;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeerReporter.AzureQueueLibrary.QueueConnection
{
    public interface ICloudQueueService
    {
        T Read<T>(string message);
        Task SendAsync<T>(T obj) where T : BaseQueueMessage;
    }

    public class CloudQueueService : ICloudQueueService
    {
        private readonly IMessageSerializer _messageSerializer;
        private readonly ICloudQueueFactory _cloudQueueClientFactory;

        public CloudQueueService(IMessageSerializer messageSerializer, ICloudQueueFactory cloudQueueClientFactory)
        {
            this._messageSerializer = messageSerializer;
            this._cloudQueueClientFactory = cloudQueueClientFactory;
        }

        /// <summary>
        /// Read a queue message.
        /// </summary>
        public T Read<T>(string message)
        {
            return _messageSerializer.Deserialize<T>(message);
        }

        /// <summary>
        /// Send a queue message.
        /// </summary>
        public async Task SendAsync<T>(T obj) where T : BaseQueueMessage
        {
            // Get client
            var client = _cloudQueueClientFactory.GetClient();

            // Get the queue reference / queue name from the command
            var queueReference = client.GetQueueReference(obj.Route);

            // Creates the queue if it doesn't exist
            await queueReference.CreateIfNotExistsAsync();

            // Create the queueMessage
            var serializedMessage = _messageSerializer.Serialize(obj);
            var queueMessage = new CloudQueueMessage(serializedMessage);

            // Add the message to the queue for later (background) processing
            await queueReference.AddMessageAsync(queueMessage);
        }
    }
}
