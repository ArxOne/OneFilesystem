# OneFilesystem

**First of all**: this is not released yet, but will be available soon (before end or March 2015) as a NuGet package. Stay tuned.

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
    var children = oneFilesystem.EnumerateEntries("C:\MyDirectory");
    using (var stream = oneFilesystem.OpenRead("ftp://myserver/somefile.txt"))
    {
    }
}
```

### Paths

OneFilesystem works with URI (where you specify `file`, `ftp`, `ftps`, `ftpes`or `sftps` as scheme, a target host and port and a local path). You can also use plain local paths (such as `C:\Windows`)

All of these path kinds are handled in a unique way by a unique class which is `OnePath`.
Fortunately for you (and for me), there are implicit conversions from `string`or `Uri`.

### Operations

A very few and simple operations are supported (extending them is planned, but it will stay simple anyway):
 * Enumerating directory entries using `EnumerateEntries`
 * Getting details about one file using `GetInformation`
 * Open a file for reading using `OpenRead`
 * Create a file for writing using `CreateFile`
 * Create a directory using `CreateDirectory`
 * Delete a file or directory using `Delete`
