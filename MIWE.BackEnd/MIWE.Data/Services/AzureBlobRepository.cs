using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services
{
    public class AzureBlobRepository : IAzureBlobRepository
    {
        private IConfiguration _configuration;
        private string _connectionString;
        public AzureBlobRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("AzureConnString").Value; ;
        }

        public async Task DownloadAsync(string blobName, string destinationPath)
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, "plugins");
            await container.CreateIfNotExistsAsync();
            // Get a reference to a blob
            BlobClient blob = container.GetBlobClient(blobName);

            BlobDownloadInfo download = await blob.DownloadAsync();
            using (FileStream file = File.OpenWrite(destinationPath))
            {
                await download.Content.CopyToAsync(file);
            }
        }

        public async Task<string> UploadAsync(string blobName, string filePath, bool overwrite = true)
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, "plugins");
            await container.CreateIfNotExistsAsync();

            // Get a reference to a blob
            BlobClient blob = container.GetBlobClient(blobName);
            bool blobExists = await blob.ExistsAsync();
            if (!blobExists || overwrite)
            {
                // Open the file and upload its data
                using (FileStream file = File.OpenRead(filePath))
                {
                    await blob.UploadAsync(file, overwrite);
                }
            }
            return blob.Uri.AbsoluteUri;
        }

        public async Task<string> UploadAsync(string blobName, Stream content, bool overwrite = true)
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, "plugins");
            await container.CreateIfNotExistsAsync();

            // Get a reference to a blob
            BlobClient blob = container.GetBlobClient(blobName);
            bool blobExists = await blob.ExistsAsync();
            if (!blobExists || overwrite)
            {
                await blob.UploadAsync(content, overwrite);
            }
            return blob.Uri.AbsoluteUri;
        }
    }
}
