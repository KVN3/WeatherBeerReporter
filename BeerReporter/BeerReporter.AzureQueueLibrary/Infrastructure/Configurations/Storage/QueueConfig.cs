using BeerReporter.AzureLibrary.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureQueueLibrary.Infrastructure
{
    public class QueueConfig : StorageConfig
    {
        public QueueConfig(string connectionString)
            : base(connectionString)
        {

        }
    }
}
