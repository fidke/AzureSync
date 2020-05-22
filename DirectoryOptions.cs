using System;
using System.Collections.Generic;
using System.Text;

namespace AzureSync
{
    public class DirectoryOptions
    {
        public string LocalPath { get; set; }
        public string RemotePath { get; set; }
        public int UploadDelay { get; set; }
        public string Filter { get; set; }
    }
}
