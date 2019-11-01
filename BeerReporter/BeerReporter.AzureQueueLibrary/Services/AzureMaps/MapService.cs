using AzureMapsToolkit;
using AzureMapsToolkit.Render;
using AzureMapsToolkit.Search;
using BeerReporter.AzureLibrary.Converter;
using BeerReporter.AzureLibrary.Infrastructure;
using BeerReporter.AzureLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeerReporter.AzureLibrary.Maps
{
    public interface IMapService
    {
        ImageFile GetMapImage(double lon, double lat, int zoom, string blobName);
        SearchAddressResult SearchAddress(string location);
    }

    public class MapService : IMapService
    {
        private readonly MapsConfig config;
        private readonly IImageHelper imageHelper;

        public MapService(MapsConfig config, IImageHelper imageHelper)
        {
            this.config = config;
            this.imageHelper = imageHelper;
        }

        public SearchAddressResult SearchAddress(string location)
        {
            SearchAddressResult result = null;

            var am = new AzureMapsToolkit.AzureMapsServices(config.Key);
            var searchAddressRequest = new SearchAddressRequest
            {
                Query = location,
                Limit = 10
            };

            var resp = am.GetSearchAddress(searchAddressRequest).Result;

            if (resp.Error == null && resp.Result.Results.Length > 0)
                result = resp.Result.Results[0];

            return result;
        }

        public ImageFile GetMapImage(double lon, double lat, int zoom, string blobName)
        {
            var am = new AzureMapsToolkit.AzureMapsServices(config.Key);
            var req = new MapImageRequest
            {
                Format = AzureMapsToolkit.Render.RasterTileFormat.png,
                Layer = StaticMapLayer.basic,
                Zoom = zoom,
                Center = $"{lon},{lat}"
            };

            // Fetch the image
            Response<byte[]> content = am.GetMapImage(req).Result;
            byte[] bytes = content.Result;

            return new ImageFile(blobName, bytes, "image/png");
        }
    }
}
