using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureSync
{
    public interface ICloudStorageService
    {
        Task UploadAsync(string localPath, string remotePath);
    }
}
