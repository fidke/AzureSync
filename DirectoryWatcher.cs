﻿using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Permissions;
using Microsoft.Extensions.Logging;

namespace AzureSync
{
    public class DirectoryWatcher : IDisposable
    {
        private readonly ILogger logger;
        private readonly FileSystemWatcher watcher;
        private readonly DirectoryOptions options;
        private readonly ICloudStorageService cloudStorageService;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public DirectoryWatcher(DirectoryOptions options, ICloudStorageService cloudStorageService, 
            ILogger logger)
        {
            this.logger = logger;
            this.options = options;
            this.cloudStorageService = cloudStorageService;

            if(!Directory.Exists(options.LocalPath))
            {
                Directory.CreateDirectory(options.LocalPath);
            }

            watcher = new FileSystemWatcher();
            watcher.Path = options.LocalPath;
            watcher.NotifyFilter = NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;
            watcher.Changed += OnChanged;

            if(options.Filters != null)
            {
                foreach (var filter in options.Filters)
                {
                    watcher.Filters.Add(filter);
                }
            }

            watcher.EnableRaisingEvents = true;

            this.logger.LogInformation($"Started watching directory {options.LocalPath}. Files will be uploaded to {options.RemotePath}");
        }

        ~DirectoryWatcher()
        {
            watcher.Dispose();
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

            // ignore directories
            if(Directory.Exists(e.FullPath))
            {
                if(logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug($"Ignoring {e.FullPath} because it's a directory");
                }

                return;
            }

            ThreadPool.QueueUserWorkItem(Upload, e, true);
        }

        private async void Upload(FileSystemEventArgs e)
        {
            if(options.UploadDelay > 0)
            {
                await Task.Delay(options.UploadDelay);
            }
            
            await cloudStorageService.UploadAsync(e.FullPath, $"{options.RemotePath}/{e.Name}");
        }


        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
