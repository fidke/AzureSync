using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace AzureSync
{
    public class AzureCloudStorageService : ICloudStorageService
    {
        private readonly BlobContainerClient containerClient;
        private readonly AzureSyncOptions options;
        private readonly ILogger<AzureCloudStorageService> logger;

        public AzureCloudStorageService(ILogger<AzureCloudStorageService> logger, IOptions<AzureSyncOptions> options)
        {
            this.logger = logger;
            this.options = options.Value;

            containerClient = new BlobServiceClient(this.options.ConnectionString)
                .GetBlobContainerClient(this.options.ContainerName);
        }

        public async Task UploadAsync(string localPath, string remotePath)
        {
            try
            {
                var blobClient = containerClient.GetBlobClient(remotePath);
                using (var uploadStream = File.OpenRead(localPath))
                {
                    await blobClient.UploadAsync(uploadStream, true);
                }

                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug($"Uploaded {localPath} to {remotePath}");
                }
            }
            catch(Exception e)
            {
                logger.LogError($"Error uploading {localPath} to {remotePath}", e);
            }

        }
    }
}
