using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureSync
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly AzureSyncOptions options;
        private readonly ICollection<DirectoryWatcher> watchers;
        private readonly ICloudStorageService cloudStorageService;

        public Worker(ILogger<Worker> logger, IOptions<AzureSyncOptions> options, ICloudStorageService cloudStorageService)
        {
            this.logger = logger;
            this.options = options.Value;
            this.cloudStorageService = cloudStorageService;

            watchers = new List<DirectoryWatcher>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var directoryOption in options.Directories)
            {
                watchers.Add(new DirectoryWatcher(directoryOption, cloudStorageService, logger));
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
