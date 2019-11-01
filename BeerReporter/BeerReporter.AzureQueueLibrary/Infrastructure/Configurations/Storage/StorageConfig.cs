using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureLibrary.Infrastructure.Configurations
{
    public abstract class StorageConfig
    {
        public string StorageConnectionString { get; set; }

        public StorageConfig()
        {

        }

        public StorageConfig(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Configuration must contain StorageConnectionString");

            this.StorageConnectionString = connectionString;
        }
    }
}
