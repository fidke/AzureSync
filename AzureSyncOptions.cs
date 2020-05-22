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
}
