using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureLibrary.Infrastructure.Configurations
{
    public class OpenWeatherConfig
    {
        public string Key { get; set; }

        public readonly string BaseUrl = "http://api.openweathermap.org/";

        public OpenWeatherConfig()
        {

        }

        public OpenWeatherConfig(string key)
        {
            this.Key = key;
        }
    }
}
