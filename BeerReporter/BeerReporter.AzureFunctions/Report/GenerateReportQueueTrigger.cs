using System;
using System.Threading.Tasks;
using BeerReporter.AzureQueueLibrary.Infrastructure;
using BeerReporter.AzureQueueLibrary.QueueConnection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using BeerReporter.AzureFunctions.Infrastructure;
using BeerReporter.AzureQueueLibrary.MessageSerializer;
using BeerReporter.AzureQueueLibrary.Messages;
using BeerReporter.AzureFunctions.Report;

namespace BeerReporter.AzureFunctions
{
    public static class GenerateReportQueueTrigger
    {
        [FunctionName("GenerateReportQueueTrigger")]
        public static async Task Run(
            [QueueTrigger(Routes.QueueStorageReports, Connection = "AzureWebJobsStorage")]
            string message,
            ILogger log)
        {
            try
            {
                log.LogInformation($"C# Queue trigger function processed: {message}");

                // Services
                IMessageSerializer serializer = DIContainer.Instance.GetService<IMessageSerializer>();
                IGenerateBeerReportCommandHandler commandHandler = DIContainer.Instance.GetService<IGenerateBeerReportCommandHandler>();

                // Deserialize the message, then process the command
                GenerateBeerReportCommand command = serializer.Deserialize<GenerateBeerReportCommand>(message);
                await commandHandler.Process(command);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Something went wrong with the GenerateReportQueueTrigger: {message}");
                throw;
            }
        }
    }
}
