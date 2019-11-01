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
        /// <summary>
        /// Writes text on an image.
        /// </summary>
        /// <param name="imageStream">The image.</param>
        /// <param name="fontSize">Font size to display.</param>
        /// <param name="texts">Texts to write on the image, including x,y pixel positions.</param>
        /// <returns>Updated image.</returns>
        public Stream AddTextToImage(Stream imageStream, int fontSize, params (string text, (float x, float y) position)[] texts)
        {
            // High fontsize can cause errors
            if (fontSize > 20)
                fontSize = 20;

            var memoryStream = new MemoryStream();

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

            return memoryStream;
        }

        /// <summary>
        /// Converts a stream to an array of bytes.
        /// </summary>
        public byte[] ToByteArray(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Converts an array of bytes to a stream.
        /// </summary>
        public Stream ToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }
}
