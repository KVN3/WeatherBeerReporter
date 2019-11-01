using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureLibrary.Infrastructure
{
    public class MapsConfig
    {
        public string Key { get; set; }

        public MapsConfig()
        {

        }

        public MapsConfig(string key)
        {
            this.Key = key;
        }
    }
}
