using BeerReporter.AzureLibrary.Converter;
using BeerReporter.AzureLibrary.Infrastructure;
using BeerReporter.AzureLibrary.Maps;
using BeerReporter.AzureLibrary.Services.Storage.Blob;
using BeerReporter.AzureQueueLibrary.MessageSerializer;
using BeerReporter.AzureQueueLibrary.QueueConnection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Http;
using BeerReporter.AzureLibrary.Services.OpenWeatherMap;
using BeerReporter.AzureLibrary.Infrastructure.Configurations;
using BeerReporter.AzureLibrary.Helpers;

namespace BeerReporter.AzureQueueLibrary.Infrastructure
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddAzureLibrary(this IServiceCollection services, 
            string storageConnString, 
            string azureMapsKey,
            string openWeatherKey)
        {
            services.AddSingleton<IMessageSerializer, JsonMessageSerializer>();

            // Queue
            services.AddSingleton(new QueueConfig(storageConnString));
            services.AddSingleton<ICloudQueueFactory, CloudQueueFactory>();
            services.AddTransient<ICloudQueueService, CloudQueueService>();

            // Blob
            services.AddSingleton(new BlobConfig(storageConnString));
            services.AddSingleton<ICloudBlobFactory, CloudBlobFactory>();
            services.AddTransient<ICloudBlobService, CloudBlobService>();

            // Maps
            services.AddSingleton(new MapsConfig(azureMapsKey));
            services.AddSingleton<IMapService, MapService>();

            // Helpers
            services.AddSingleton<IImageHelper, ImageHelper>();
            services.AddSingleton<ITemperatureConverter, TemperatureConverter>();

            // HttpClients / APIs
            services.AddSingleton(new OpenWeatherConfig(openWeatherKey));
            services.AddHttpClient<IOpenWeatherService, OpenWeatherService>();

            //Microsoft.Extensions.Http
            return services;
        }
    }
}