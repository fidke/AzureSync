using System;
using System.Collections.Generic;
using System.Text;

namespace AzureSync
{
    public class AzureSyncOptions
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public IEnumerable<DirectoryOptions> Directories { get; set; }
    }

    public class DirectoryOptions
    {
        public string LocalPath { get; set; }
        public string RemotePath { get; set; }
        public int UploadDelay { get; set; }
        public IEnumerable<string> Filters { get; set; }
    }
}
