# OneFilesystem

**First of all**: it is released as a [NuGet package](https://www.nuget.org/packages/OneFilesystem/). **Important too**: currently it is a pre-release package, because it depends on an [SSH.NET package](https://www.nuget.org/packages/SSH.NET/) version which is prele-release too. Fortunately, things will change soon.

## Why another library of this kind?

Because I found two kind of libraries:
 * The complex kind, which do a lot, but not in a very simple way.
 * The non-functional kind, which are simple, but do not exactly work.

## What is does

Unified filesystem to handle local files, `FTP[[E]S]` and `SFTP`.

## How it does

### In a nutshell

You just need to instantiate a `OneFilesystem`. It contains a very few functions (detailed below).
For example:
```csharp
using (var oneFilesystem = new OneFilesystem())
{
    // here, we read some files
    var children = oneFilesystem.GetChildren("C:\MyDirectory");
    // now, we create another
    using (var stream = oneFilesystem.OpenRead("ftp://myserver/somefile.txt"))
    {
    }
}
```

### Paths

OneFilesystem works with URI (where you specify `"file"`, `"ftp"`, `"ftps"`, `"ftpes"`or `"sftps"` as scheme, a target host and port and a local path). You can also use plain local paths (such as `"C:\Windows"`)

All of these path kinds are handled in a unique way by a unique class which is `OnePath`.
Fortunately for you (and for me), there are implicit conversions from `string`or `Uri`.

### Operations

A very few and simple operations are supported (extending them is planned, but it will stay simple anyway):
 * Enumerating directory entries using `GetChildren`
 * Getting details about one file using `GetInformation`
 * Open a file for reading using `OpenRead`
 * Create a file for writing using `CreateFile`
 * Create a directory using `CreateDirectory`
 * Delete a file or directory using `Delete`
