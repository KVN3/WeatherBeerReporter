using BeerReporter.AzureFunctions.Report;
using BeerReporter.AzureQueueLibrary.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeerReporter.AzureFunctions.Infrastructure
{
    public sealed class DIContainer
    {
        private static readonly IServiceProvider _instance = Build();
        public static IServiceProvider Instance => _instance;

        static DIContainer()
        {

        }

        private DIContainer()
        {

        }

        private static IServiceProvider Build()
        {
            var services = new ServiceCollection();

            // Cloud uses environment variables, local uses local.settings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Here we will add the different services, such as API's, queue / blob library, etc.
            // API 1
            // API 2
            services.AddSingleton<IGenerateBeerReportCommandHandler, GenerateBeerReportCommandHandler>();
            services.AddAzureLibrary(configuration["AzureWebJobsStorage"], 
                configuration["AzureMapsKey"], configuration["OpenWeatherKey"]);
            // Blob library

            return services.BuildServiceProvider();
        }
    }
}
