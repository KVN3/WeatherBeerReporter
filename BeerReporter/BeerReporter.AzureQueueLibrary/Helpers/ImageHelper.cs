using System;
using System.Collections.Generic;

using System.IO;
using SixLabors.Fonts;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp.Processing.Processors.Text;

namespace BeerReporter.AzureLibrary.Converter
{
    public interface IImageHelper
    {
        Stream ToStream(byte[] bytes);
        byte[] ToByteArray(Stream stream);
        Stream AddTextToImage(Stream imageStream, int fontSize, params (string text, (float x, float y) position)[] texts);
    }

    public class ImageHelper : IImageHelper
    {
        public Stream AddTextToImage(Stream imageStream, int fontSize, params (string text, (float x, float y) position)[] texts)
        {
            //Stream imageStreamCopy = new MemoryStream();
            //imageStream.CopyTo(imageStreamCopy);

            var memoryStream = new MemoryStream();
            bool hasFailed = false;

            try
            {
                var image = Image.Load(imageStream);

                image
                    .Clone(img =>
                    {
                        foreach (var (text, (x, y)) in texts)
                        {
                            img.DrawText(text, SystemFonts.CreateFont("Verdana", fontSize), Rgba32.OrangeRed, new PointF(x, y));
                        }
                    })
                    .SaveAsPng(memoryStream);

                memoryStream.Position = 0;
            }
            catch (Exception ex)
            {
                hasFailed = true;
            }

            // Try to write with a smaller fontSize, as long as it's above 12px
            //if (hasFailed && fontSize > 12)
            //    return AddTextToImage((Stream)imageStreamCopy, fontSize / 2, texts);
            //else
            return memoryStream;
        }

        public byte[] ToByteArray(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public Stream ToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }
}
