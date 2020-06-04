# AzureSync
Simple windows service that watches configured directories and uploads new files to [Azure blob storage](https://azure.microsoft.com/en-us/services/storage/blobs/).

## Installation
#### Create the windows service
```
sc create "AzureSync" binPath="c:/path/to/AzureSync/AzureSync.exe"
```

## Configuration
Edit `c:/path/to/AzureSync/appsettings.json` and change the following:
  - `ConnectionString`: Azure blob storage connection string. Get this from the [Azure Portal](https://portal.azure.com)
  - `ContainerName`: Azure blob storage container name
  - `Directories`: Mappings of local directories to virtual directories under the blob container


Sample configuration:
```
"AzureSync": {
    "ConnectionString": "<YOUR CONNECTION STRING>",
    "ContainerName": "cameras",
    "Directories": [
      {
        "LocalPath": "d:/cameras/video/front001",
        "Filter": "*.mp4",
        "RemotePath": "front001",
        "UploadDelay": 500
      },
      {
        "LocalPath": "d:/cameras/video/front002",
        "Filter": "*.mp4",
        "RemotePath": "front002",
        "UploadDelay": 500
      }
    ]
  }
```


## References
#### .net Core windows services
  - https://csharp.christiannagel.com/2019/10/15/windowsservice/
  - https://dotnetcoretutorials.com/2019/12/07/creating-windows-services-in-net-core-part-3-the-net-core-worker-way/

#### Windows service management using sc.exe
  - https://docs.microsoft.com/en-us/windows/win32/services/controlling-a-service-using-sc

#### FileSystemWatcher
  - https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=netcore-3.1
