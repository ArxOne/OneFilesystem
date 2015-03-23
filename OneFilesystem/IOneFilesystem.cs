#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem
{
    using System.Collections.Generic;
    using System.IO;

    public interface IOneFilesystem
    {
        /// <summary>
        /// Enumerates the entries.
        /// </summary>
        /// <param name="directoryPath">A directory path to get listing from</param>
        /// <returns>A list, or null if the directory is not found (if the directoryPath points to a file, an empty list is returned)</returns>
        IEnumerable<OneEntryInformation> GetChildren(OnePath directoryPath);

        /// <summary>
        /// Gets the information about the referenced file.
        /// </summary>
        /// <param name="entryPath">A file path to get information about</param>
        /// <returns>Information or null if entry is not found</returns>
        OneEntryInformation GetInformation(OnePath entryPath);

        /// <summary>
        /// Opens file for reading.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>A readable stream, or null if the file does not exist or is a directory</returns>
        Stream OpenRead(OnePath filePath);

        /// <summary>
        /// Deletes the specified file or directory (does not recurse directories).
        /// </summary>
        /// <param name="entryPath"></param>
        /// <returns>true is entry was successfully deleted</returns>
        bool Delete(OnePath entryPath);

        /// <summary>
        /// Creates the file and returns a writable stream.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>A writable stream or null if creation fails (entry exists or path not found)</returns>
        Stream CreateFile(OnePath filePath);

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>true if directory was created</returns>
        bool CreateDirectory(OnePath directoryPath);
    }
}
