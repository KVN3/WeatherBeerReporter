using BeerReporter.AzureLibrary.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureLibrary.Infrastructure
{
    public class BlobConfig : StorageConfig
    {

        public BlobConfig(string connectionString)
            : base(connectionString)
        {

        }
    }
}
