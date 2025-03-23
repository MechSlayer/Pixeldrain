# Pixeldrain .NET API Client

[![NuGet](https://img.shields.io/nuget/v/Pixeldrain.svg)](https://www.nuget.org/packages/Pixeldrain/)
[![Downloads](https://img.shields.io/nuget/dt/Pixeldrain.svg)](https://www.nuget.org/packages/Pixeldrain/)
[![License](https://img.shields.io/github/license/MechSlayer/Pixeldrain)](https://github.com/MechSlayer/Pixeldrain/blob/main/LICENSE)

A modern .NET client library for the [Pixeldrain](https://pixeldrain.com) file hosting service API.

## Features

- Upload files with progress reporting
- Download files with progress reporting
- Rename files
- Delete files
- Fully asynchronous API
- Cancellation support
- Strong typing and exception handling

## Installation

```
dotnet add package Pixeldrain
```

## Usage Examples

### Upload a File

```csharp
using Pixeldrain;
using Pixeldrain.API;

var client = new PixeldrainClient("<api-key>");
var filesApi = client.Files;

// Simple upload
string fileId = await filesApi.UploadAsync("example.txt", File.OpenRead("path/to/file.txt"));

// Upload with progress reporting
var progress = new Progress<long>(bytesUploaded => 
    Console.WriteLine($"Uploaded {bytesUploaded} bytes"));

string fileId = await filesApi.UploadAsync(
    "example.txt", 
    File.OpenRead("path/to/file.txt"),
    progress);
```

### Download a File

```csharp
// Simple download
await filesApi.DownloadAsync(fileId, File.Create("path/to/destination.txt"));

// Download with progress reporting
var progress = new Progress<long>(bytesDownloaded => 
    Console.WriteLine($"Downloaded {bytesDownloaded} bytes"));

await filesApi.DownloadAsync(
    fileId, 
    File.Create("path/to/destination.txt"),
    progress);
```

### Other Operations

```csharp
// Rename a file
await filesApi.RenameAsync(fileId, "new-name.txt");

// Delete a file
await filesApi.DeleteAsync(fileId);
```

## Requirements

- .NET 9.0 or later

## License

This project is licensed under the MIT License - see the LICENSE file for details.