using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureQueueLibrary.Messages
{
    /// <summary>
    /// A command we're writing to the queue.
    /// </summary>
    public abstract class BaseQueueMessage
    {
        public string Route { get; set; }

        public BaseQueueMessage(string route)
        {
            this.Route = route;
        }

    }
}
