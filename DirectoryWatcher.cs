using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Security.Permissions;

namespace AzureSync
{
    public class DirectoryWatcher : IDisposable
    {
        private readonly ILogger logger;
        private readonly FileSystemWatcher watcher;
        private readonly DirectoryOptions options;
        private readonly BlobContainerClient containerClient;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public DirectoryWatcher(DirectoryOptions options, BlobContainerClient containerClient, ILogger logger)
        {
            this.logger = logger;
            this.options = options;
            this.containerClient = containerClient;

            watcher = new FileSystemWatcher();
            watcher.Path = options.LocalPath;
            watcher.NotifyFilter = NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;
            watcher.Changed += OnChanged;

            if(!string.IsNullOrEmpty(options.Filter))
            {
                watcher.Filter = options.Filter;
            }
            
            watcher.EnableRaisingEvents = true;

            this.logger.LogInformation($"Started watching directory {options.LocalPath}. Files will be uploaded to {options.RemotePath}");
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if(logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"Changed: {e.FullPath} {e.ChangeType} {DateTimeOffset.Now}");
            }
            
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug($"Ignoring {e.ChangeType} on {e.FullPath}");
                }
                    
                return;
            }

            ThreadPool.QueueUserWorkItem(Upload, e, true);
        }

        private async void Upload(FileSystemEventArgs e)
        {
            await Task.Delay(options.UploadDelay);

            try
            {
                var blobClient = containerClient.GetBlobClient($"{options.RemotePath}/{e.Name}");
                using (var uploadStream = File.OpenRead(e.FullPath))
                {
                    await blobClient.UploadAsync(uploadStream, true);
                }

                if(logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug($"Uploaded {e.FullPath} to {options.RemotePath}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error uploading {e.FullPath} to {options.RemotePath}", ex);
            }
        }


        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
