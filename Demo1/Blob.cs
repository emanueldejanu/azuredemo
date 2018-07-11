using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo1
{
    public class Blob
    {
        string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=alexblobstorage1992;AccountKey=7XW6chCYwfsJpyg7QKHxd8vdjkDQ9IAuyv4rkIwl0FmGl2ByxlmIPIgtO3tEV1QGoVZke/TQ4QprIzoc2+S/Fw==;EndpointSuffix=core.windows.net";
        static CloudBlobClient blobClient;
        const string blobContainerName = "alex";
        static CloudBlobContainer blobContainer;
        public async Task<List<IListBlobItem>> GetAllBlobs()
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference(blobContainerName);
                await blobContainer.CreateIfNotExistsAsync();

                await blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });


                BlobContinuationToken continuationToken = null;
                List<IListBlobItem> blobItems = new List<IListBlobItem>();

                do
                {
                    var response = await blobContainer.ListBlobsSegmentedAsync(continuationToken);
                    continuationToken = response.ContinuationToken;
                    blobItems.AddRange(response.Results);
                }
                while (continuationToken != null);

                return blobItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UploadBlob(string blobContainer, IFormFile file)
        {
            //int fileNameStartLocation = file.FileName.LastIndexOf("\\") + 1;
            string fileName = Guid.NewGuid().ToString();

            CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
            await container.CreateIfNotExistsAsync();

            await container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            MagickImage image = new MagickImage(file.OpenReadStream());
            image.AutoOrient();

            await memoryStream.WriteAsync(image.ToByteArray(), 0, image.ToByteArray().Length);
            memoryStream.Position = 0;

            await blockBlob.UploadFromStreamAsync(memoryStream);
        }
    }
}


