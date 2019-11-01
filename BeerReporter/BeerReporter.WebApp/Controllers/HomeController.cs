using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeerReporter.WebApp.Models;
using BeerReporter.AzureQueueLibrary.QueueConnection;
using BeerReporter.AzureQueueLibrary.Messages;
using BeerReporter.AzureLibrary.Maps;
using BeerReporter.AzureLibrary.Services.Storage.Blob;
using BeerReporter.AzureLibrary.Models;
using AzureMapsToolkit.Search;
using BeerReporter.AzureLibrary.Converter;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;
using BeerReporter.AzureLibrary.Services.OpenWeatherMap;

namespace BeerReporter.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOpenWeatherService service;
        private readonly ICloudQueueService _queueCommunicator;
        private readonly IMapService mapServices;
        private readonly IImageHelper imageHelper;
        private readonly ICloudBlobService _cloudBlobService;

        public HomeController(IOpenWeatherService service, ICloudQueueService queueCommunicator, IMapService mapServices, IImageHelper imageHelper, ICloudBlobService cloudBlobService)
        {
            this.service = service;
            this._queueCommunicator = queueCommunicator;
            this.mapServices = mapServices;
            this.imageHelper = imageHelper;
            this._cloudBlobService = cloudBlobService;
        }


        public IActionResult Index()
        {
            ReportViewModel model = new ReportViewModel();
            model.RemoveNulls();

            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Index(string location)
        {
            // No location passed
            if (string.IsNullOrEmpty(location))
            {
                ViewBag.Message = $"Empty location input... please input valid location.";
                return View();
            }

            // Location not found
            if (mapServices.SearchAddress(location) == null)
            {
                ViewBag.Message = $"Couldn't find address for location: {location}. Please try again with a different location.";
                return View();
            }

            // Prepare the queue command
            var queueCommand = new GenerateBeerReportCommand()
            {
                BlobName = location + _cloudBlobService.GetRandomBlobName(location + ".png"),
                Location = location
            };

            // Get and save the sas uri so we can access the blob once it's done
            string sasUri = await _cloudBlobService.GetBlobSasUri(queueCommand.BlobName);
            HttpContext.Session.SetString("sas_uri", sasUri);

            // Send to the queue
            await _queueCommunicator.SendAsync(queueCommand);

            ViewBag.Message = $"Thank you. Request for beer report in {location} will be added to queue.";
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GenerateReport(ReportViewModel model)
        {
            model.RemoveNulls();

            // No location passed
            if (string.IsNullOrEmpty(model.Location))
            {
                model.GenerateMessage = $"Empty location input... please input valid location.";
                return View("Index", model);
            }

            // Location not found
            if (mapServices.SearchAddress(model.Location) == null)
            {
                model.GenerateMessage = $"Couldn't find address for location: {model.Location}. Please try again with a different location.";
                return View("Index", model);
            }

            // Prepare the queue command
            var queueCommand = new GenerateBeerReportCommand()
            {
                BlobName = model.Location + _cloudBlobService.GetRandomBlobName(model.Location + ".png"),
                Location = model.Location
            };

            // Get and save the sas uri so we can access the blob once it's done
            string sasUri = await _cloudBlobService.GetBlobSasUri(queueCommand.BlobName);
            HttpContext.Session.SetString("sas_uri", sasUri);
            HttpContext.Session.SetString("blob_name", queueCommand.BlobName);

            // Send to the queue
            await _queueCommunicator.SendAsync(queueCommand);

            model.GenerateMessage = $"Thank you. Request for beer report in {model.Location} will be added to queue.";
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> FetchReport(ReportViewModel model)
        {
            model.RemoveNulls();

            if (await _cloudBlobService.IsBlobReady(HttpContext.Session.GetString("blob_name")))
            {
                model.SasUri = HttpContext.Session.GetString("sas_uri");
                model.FetchMessage = "Fetched!";
            }
            else
            {
                model.FetchMessage = "Blob not ready yet... try again later.";
            }

            return View("Index", model);
        }
    }
}
