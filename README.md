# AzureSync
Simple windows service that watches configured directories and uploads new files to [Azure blob storage](https://azure.microsoft.com/en-us/services/storage/blobs/).

## Installation
#### Create the windows service
```
sc create "AzureSync" binPath="c:/path/to/AzureSync/AzureSync.exe"
```

#### Start/stop the service
Run the **Serivces** app (Windows Services Manager)

## Configuration
Edit `c:/path/to/AzureSync/appsettings.json` and update the following:
  - `ConnectionString`: Azure blob storage connection string. Get this from the [Azure Portal](https://portal.azure.com)
  - `ContainerName`: Azure blob storage container name
  - `Directories`: Mappings of local directories to virtual directories under the blob container
    - `LocalPath`: Full path to the local directory that should be monitored for new files. eg `d:/cameras/video/front001`
    - `RemotePath`: Virtual path under `ContainerName` in Azure blob storage
    - `Filters` (optional): File name pattern filters for `LocalPath`. See [FileSystemWatcher.Filters](https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher.filters?view=netcore-3.1) and [FileSystemWatcher.Filter](https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher.filter?view=netcore-3.1). Leave this unset to upload any new file created in the local directory.
    - `UploadDelay` (optional): Time to wait in ms before uploading the new file to Azure blob storage. Leave this unset to upload immediately after the new file has been created.

Sample configuration:
```
"AzureSync": {
    "ConnectionString": "<YOUR CONNECTION STRING>",
    "ContainerName": "cameras",
    "Directories": [
      {
        "LocalPath": "d:/cameras/video/front001",
        "Filters": [ "*.mp4", "*.mkv" ],
        "RemotePath": "front001",
        "UploadDelay": 500
      },
      {
        "LocalPath": "d:/cameras/video/front002",
        "RemotePath": "front002"
      }
    ]
  }
```

## Troubleshooting
1. Run **Event Viewer**
2. Check logs under **Windows Logs -> Application** and **Applications and Services Logs -> AzureSync**

## References
#### .net Core windows services
  - https://csharp.christiannagel.com/2019/10/15/windowsservice/
  - https://dotnetcoretutorials.com/2019/12/07/creating-windows-services-in-net-core-part-3-the-net-core-worker-way/

#### Windows service management using sc.exe
  - https://docs.microsoft.com/en-us/windows/win32/services/controlling-a-service-using-sc

#### FileSystemWatcher
  - https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=netcore-3.1
