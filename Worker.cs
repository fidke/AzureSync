using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace AzureSync
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly AzureSyncOptions options;
        private readonly ICollection<DirectoryWatcher> watchers;

        public Worker(ILogger<Worker> logger, IOptions<AzureSyncOptions> options)
        {
            this.logger = logger;
            this.options = options.Value;
            watchers = new List<DirectoryWatcher>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var blobServiceClient = new BlobServiceClient(options.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(options.ContainerName);

            foreach (var directoryOption in options.Directories)
            {
                watchers.Add(new DirectoryWatcher(directoryOption, containerClient, logger));
            }

            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100, stoppingToken);
            }

            foreach(var watcher in watchers)
            {
                watcher.Dispose();
            }
        }
    }
}
