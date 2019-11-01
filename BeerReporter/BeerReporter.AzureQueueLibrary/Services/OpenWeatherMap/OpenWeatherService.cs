using BeerReporter.AzureLibrary.Infrastructure.Configurations;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Text;
using BeerReporter.AzureLibrary.Models;
using System.Threading.Tasks;
using BeerReporter.AzureQueueLibrary.MessageSerializer;
using System.Net;

namespace BeerReporter.AzureLibrary.Services.OpenWeatherMap
{
    public interface IOpenWeatherService
    {
        Task<WeatherReport> GetWeatherReport(double latitude, double longitude);
    }

    public class OpenWeatherService : IOpenWeatherService
    {
        private readonly OpenWeatherConfig config;
        private readonly HttpClient _httpClient;
        private readonly IMessageSerializer _messageSerializer;

        public OpenWeatherService(OpenWeatherConfig config, HttpClient httpClient, IMessageSerializer messageSerializer)
        {
            this.config = config;
            this._httpClient = httpClient;
            this._messageSerializer = messageSerializer;

            _httpClient.BaseAddress = new Uri(config.BaseUrl);
        }

        public async Task<WeatherReport> GetWeatherReport(double latitude, double longitude)
        {
            var responseString = await _httpClient.GetStringAsync(
                $"data/2.5/weather?lat={latitude}&lon={longitude}&APPID={config.Key}&units=metric");

            return _messageSerializer.Deserialize<WeatherReport>(responseString);
        }
    }
}
