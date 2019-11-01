using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureQueueLibrary.MessageSerializer
{
    public class JsonMessageSerializer : IMessageSerializer
    {
        /// <summary>
        /// JSON to Object.
        /// </summary>
        public T Deserialize<T>(string message)
        {
            var obj = JsonConvert.DeserializeObject<T>(message);
            return obj;
        }

        /// <summary>
        /// Object to JSON.
        /// </summary>
        public string Serialize(object obj)
        {
            var message = JsonConvert.SerializeObject(obj);
            return message;
        }
    }
}
