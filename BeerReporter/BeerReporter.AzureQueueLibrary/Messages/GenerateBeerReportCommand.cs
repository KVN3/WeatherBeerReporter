using BeerReporter.AzureQueueLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureQueueLibrary.Messages
{
    public class GenerateBeerReportCommand : BaseQueueMessage
    {
        public string BlobName { get; set; }
        public string Location { get; set; }

        public GenerateBeerReportCommand() 
            : base(Routes.QueueStorageReports)
        {
        }
    }
}
