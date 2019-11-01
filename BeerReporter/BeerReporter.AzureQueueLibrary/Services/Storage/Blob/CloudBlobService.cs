using BeerReporter.AzureLibrary.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BeerReporter.AzureLibrary.Services.Storage.Blob
{
    public interface ICloudBlobService
    {
        string PolicyName { get; set; }
        Task<CloudBlockBlob> UploadAsync(ImageFile imageFile);
        Task<string> GetBlobSasUri(string fileName);
        Task<bool> IsBlobReady(string sasUri);

        string GetRandomBlobName(string filename);
    }

    public class CloudBlobService : ICloudBlobService
    {
        public string PolicyName { get; set; }
        private readonly ICloudBlobFactory _cloudBlobFactory;

        public CloudBlobService(ICloudBlobFactory cloudBlobFactory)
        {
            this._cloudBlobFactory = cloudBlobFactory;
        }

        public async Task<CloudBlockBlob> UploadAsync(ImageFile imageFile)
        {
            var blobContainer = await _cloudBlobFactory.GetBlobContainer();

            // If blobname hasn't been passed along, make a new random one
            if (imageFile.BlobName == null)
                imageFile.BlobName = GetRandomBlobName(imageFile.ContentType);

            // Get the blob we're looking for
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(imageFile.BlobName);

            // Set its content type
            blob.Properties.ContentType = imageFile.ContentType;

            // Upload the blob
            await blob.UploadFromByteArrayAsync(imageFile.Bytes, 0, imageFile.Bytes.Length);

            return blob;
        }

        public async Task<string> GetBlobSasUri(string blobName)
        {
            var blobContainer = await _cloudBlobFactory.GetBlobContainer();
            string sasBlobToken;

            // Get a reference to a blob within the container.
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);

            if (PolicyName == null)
            {
                // Create a new access policy and define its constraints.
                // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad hoc SAS, and
                // to construct a shared access policy that is saved to the container's shared access policies.
                SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
                };

                // Generate the shared access signature on the blob, setting the constraints directly on the signature.
                sasBlobToken = blob.GetSharedAccessSignature(adHocSAS);

                Console.WriteLine("SAS for blob (ad hoc): {0}", sasBlobToken);
                Console.WriteLine();
            }
            else
            {
                // Generate the shared access signature on the blob. In this case, all of the constraints for the
                // shared access signature are specified on the container's stored access policy.
                sasBlobToken = blob.GetSharedAccessSignature(null, PolicyName);

                Console.WriteLine("SAS for blob (stored access policy): {0}", sasBlobToken);
                Console.WriteLine();
            }

            // Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }

        /// <summary> 
        /// string GetRandomBlobName(string filename): Generates a unique random file name to be uploaded  
        /// </summary> 
        public string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }

        public async Task<bool> IsBlobReady(string blobName)
        {
            var blobContainer = await _cloudBlobFactory.GetBlobContainer();

            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);
            bool exists = await blob.ExistsAsync();

            return exists;
        }
    }
}
