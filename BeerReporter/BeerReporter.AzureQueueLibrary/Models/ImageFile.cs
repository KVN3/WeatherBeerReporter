using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureLibrary.Models
{
    public struct ImageFile
    {
        public ImageFile(string blobName, byte[] bytes, string extension)
        {
            BlobName = blobName;
            Bytes = bytes;
            ContentType = extension;
        }

        public string BlobName { get; set; }
        public byte[] Bytes { get; set; }
        public string ContentType { get; set; }
    }
}
