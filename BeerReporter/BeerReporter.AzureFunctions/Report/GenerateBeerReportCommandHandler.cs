using AzureMapsToolkit.Search;
using BeerReporter.AzureLibrary.Converter;
using BeerReporter.AzureLibrary.Helpers;
using BeerReporter.AzureLibrary.Maps;
using BeerReporter.AzureLibrary.Models;
using BeerReporter.AzureLibrary.Services.OpenWeatherMap;
using BeerReporter.AzureLibrary.Services.Storage.Blob;
using BeerReporter.AzureQueueLibrary.Messages;
using BeerReporter.AzureQueueLibrary.QueueConnection;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BeerReporter.AzureFunctions.Report
{
    public interface IGenerateBeerReportCommandHandler
    {
        Task Process(GenerateBeerReportCommand command);
    }

    public class GenerateBeerReportCommandHandler : IGenerateBeerReportCommandHandler
    {
        private readonly float _optimalDrinkingTemperature = 25;
        private readonly int _mapZoom = 10;

        private readonly ICloudQueueService _queueCommunicator;
        private readonly ICloudBlobService _cloudBlobService;

        private readonly IMapService _mapServices;
        private readonly IOpenWeatherService _openWeatherService;

        private readonly IImageHelper _imageHelper;
        private readonly ITemperatureConverter _temperatureConverter;

        public GenerateBeerReportCommandHandler(ICloudQueueService queueCommunicator, ICloudBlobService cloudBlobService,
            IMapService mapServices, IOpenWeatherService openWeatherService, 
            IImageHelper imageHelper, ITemperatureConverter temperatureConverter)
        {
            this._queueCommunicator = queueCommunicator;
            this._mapServices = mapServices;
            this._openWeatherService = openWeatherService;
            this._imageHelper = imageHelper;
            this._temperatureConverter = temperatureConverter;
            this._cloudBlobService = cloudBlobService;
        }

        /// <summary>
        /// Processes a command.
        /// </summary>
        public async Task Process(GenerateBeerReportCommand command)
        {
            // Get the map image and forecast, then write the forecast on the image
            ImageFile imageFile = await CreateMap(command.Location, command.BlobName);
            
            // Upload the forged beer report map image
            CloudBlockBlob blob = await _cloudBlobService.UploadAsync(imageFile);
        }

        /// <summary>
        /// Creates a map with forecast info and suggests whether to drink beer there or not.
        /// </summary>
        /// <param name="location">Location query string.</param>
        /// <param name="blobName">Blob name on Azure blob storage.</param>
        /// <returns>ImageFile object with blob name.</returns>
        private async Task<ImageFile> CreateMap(string location, string blobName)
        {
            SearchAddressResult result = _mapServices.SearchAddress(location);
            ImageFile imageFile = _mapServices.GetMapImage(result.Position.Lon, result.Position.Lat, _mapZoom, blobName);

            // Add the weather report to the map
            WeatherReport weatherReport = await _openWeatherService.GetWeatherReport(result.Position.Lat, result.Position.Lon);
            imageFile.Bytes = AddReportToMap(imageFile.Bytes, weatherReport);

            return imageFile;
        }

        /// <summary>
        /// Adds information to the map (mapBytes) based off of the WeatherReport object passed.
        /// </summary>
        /// <param name="mapBytes">Map.</param>
        /// <param name="weatherReport">Weather report object for this given map.</param>
        /// <returns>Byte array of the new and edited map.</returns>
        private byte[] AddReportToMap(byte[] mapBytes, WeatherReport weatherReport)
        {
            Stream stream = _imageHelper.ToStream(mapBytes);

            string tempMessage = $"Temperature: {weatherReport.main.temp}C";
            string suggestionMessage = CreateSuggestion(weatherReport.main.temp);

            // Apply the texts
            stream = _imageHelper.AddTextToImage(
                stream,
                fontSize: 16,
                (tempMessage, (20, 420)),
                (suggestionMessage, (20, 450)));

            return _imageHelper.ToByteArray(stream);
        }

        private string CreateSuggestion(float tempInCelsius)
        {
            if (IsWeatherSufficientForBeer(tempInCelsius))
                return "You should drink beer here!";
            else
                return "You shouldn't drink beer here!";
        }

        private bool IsWeatherSufficientForBeer(float tempInCelsius)
        {
            if (tempInCelsius >= _optimalDrinkingTemperature)
                return true;
            else
                return false;
        }
    }
}
