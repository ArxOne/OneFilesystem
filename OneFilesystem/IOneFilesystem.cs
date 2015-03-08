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
        /// <param name="directoryPath"></param>
        /// <returns>A list, or null if the directory is not found (if the directoryPath points to a file, an empty list is returned)</returns>
        IEnumerable<OneEntryInformation> EnumerateEntries(OnePath directoryPath);

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <param name="entryPath"></param>
        /// <returns></returns>
        OneEntryInformation GetInformation(OnePath entryPath);

        /// <summary>
        /// Opens file for reading.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        Stream OpenRead(OnePath filePath);

        /// <summary>
        /// Deletes the specified file or directory.
        /// </summary>
        /// <param name="entryPath"></param>
        /// <returns></returns>
        bool Delete(OnePath entryPath);

        /// <summary>
        /// Creates the file and returns a writable stream.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Stream CreateFile(OnePath filePath);

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="directoryPath"></param>
        void CreateDirectory(OnePath directoryPath);
    }
}
