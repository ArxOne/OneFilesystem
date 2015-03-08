#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem.Protocols
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Default filesystem protocol
    /// </summary>
    public class FileProtocolFilesystem : IOneProtocolFilesystem
    {
        private enum NodeType
        {
            LocalComputer,
            Server,
            Default,
        }

        /// <summary>
        /// Gets the protocol.
        /// If this is null, then the Handle() method has to be called to see if the file is handled here
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public string Protocol
        {
            get { return "file"; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        { }

        /// <summary>
        /// Indicates if this filesystem handles the given protocol.
        /// </summary>
        /// <param name="entryPath">The entry path.</param>
        /// <returns></returns>
        public bool Handle(OnePath entryPath)
        {
            return entryPath.Protocol == Protocol;
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <param name="entryPath">The entry path.</param>
        /// <returns></returns>
        private static NodeType GetNodeType(OnePath entryPath)
        {
            if (entryPath.Path.Count > 0)
                return NodeType.Default;
            if (IsLocalhost(entryPath))
                return NodeType.LocalComputer;
            return NodeType.Server;
        }

        /// <summary>
        /// Determines whether the specified entry path is localhost.
        /// </summary>
        /// <param name="entryPath">The entry path.</param>
        /// <returns></returns>
        private static bool IsLocalhost(OnePath entryPath)
        {
            // TODO a real identification for localhost
            return entryPath.Host == "localhost";
        }

        /// <summary>
        /// Gets the local path.
        /// </summary>
        /// <param name="entryPath">The entry path.</param>
        /// <returns></returns>
        private static string GetLocalPath(OnePath entryPath)
        {
            var localPath = string.Join(Path.DirectorySeparatorChar.ToString(), entryPath.Path);
            if (IsLocalhost(entryPath))
                return localPath;
            return string.Format("{0}{0}{1}{0}{2}", Path.DirectorySeparatorChar, entryPath.Host, localPath);
        }

        /// <summary>
        /// Enumerates the entries.
        /// </summary>
        /// <param name="directoryPath">A directory path to get listing from</param>
        /// <returns>
        /// A list, or null if the directory is not found (if the directoryPath points to a file, an empty list is returned)
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public IEnumerable<OneEntryInformation> EnumerateEntries(OnePath directoryPath)
        {
            switch (GetNodeType(directoryPath))
            {
                case NodeType.LocalComputer:
                    return EnumerateLocalComputerEntries(directoryPath);
                case NodeType.Server:
                    return EnumerateServerEntries(directoryPath);
                case NodeType.Default:
                    return EnumerateDefaultEntries(directoryPath);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Creates the entry information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="localPath">The local path to speed up things.</param>
        /// <param name="isDirectory">if set to <c>true</c> [is directory].</param>
        /// <returns></returns>
        private static OneEntryInformation CreateEntryInformation(OnePath path, string localPath, bool isDirectory)
        {
            // -- directory
            if (isDirectory)
            {
                var directoryInfo = new DirectoryInfo(localPath);
                return new OneEntryInformation(path, true, null, directoryInfo.CreationTimeUtc, directoryInfo.LastWriteTimeUtc, directoryInfo.LastAccessTimeUtc);
            }

            // -- file
            var fileInfo = new FileInfo(localPath);
            return new OneEntryInformation(path, false, fileInfo.Length, fileInfo.CreationTimeUtc, fileInfo.LastWriteTimeUtc, fileInfo.LastAccessTimeUtc);
        }

        /// <summary>
        /// Enumerates the local computer drives.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns></returns>
        private static IEnumerable<OneEntryInformation> EnumerateLocalComputerEntries(OnePath directoryPath)
        {
            return DriveInfo.GetDrives().Select(d => CreateEntryInformation(directoryPath + d.Name, d.Name, true));
        }

        private static IEnumerable<OneEntryInformation> EnumerateServerEntries(OnePath directoryPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enumerates the entries using the defaut filesystem.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns></returns>
        private static IEnumerable<OneEntryInformation> EnumerateDefaultEntries(OnePath directoryPath)
        {
            var localPath = GetLocalPath(directoryPath) + Path.DirectorySeparatorChar;
            if (File.Exists(localPath))
                return new OneEntryInformation[0];
            if (Directory.Exists(localPath))
            {
                return Directory.EnumerateDirectories(localPath).Select(n => CreateEntryInformation(directoryPath + Path.GetFileName(n), n, true))
                    .Union(Directory.EnumerateFiles(localPath).Select(n => CreateEntryInformation(directoryPath + Path.GetFileName(n), n, false)));
            }
            return null;
        }

        /// <summary>
        /// Gets the information about the referenced file.
        /// </summary>
        /// <param name="entryPath">A file path to get information about</param>
        /// <returns>
        /// Information or null if entry is not found
        /// </returns>
        public OneEntryInformation GetInformation(OnePath entryPath)
        {
            var localPath = GetLocalPath(entryPath);
            if (Directory.Exists(localPath))
                return CreateEntryInformation(entryPath, localPath, true);
            if (File.Exists(localPath))
                return CreateEntryInformation(entryPath, localPath, false);
            return null;
        }

        /// <summary>
        /// Opens file for reading.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// A readable stream, or null if the file does not exist
        /// </returns>
        public Stream OpenRead(OnePath filePath)
        {
            var localPath = GetLocalPath(filePath);
            if (File.Exists(localPath))
                return File.OpenRead(localPath);

            return null;
        }

        /// <summary>
        /// Deletes the specified file or directory (does not recurse directories).
        /// </summary>
        /// <param name="entryPath"></param>
        /// <returns>
        /// true is entry was successfully deleted
        /// </returns>
        public bool Delete(OnePath entryPath)
        {
            var localPath = GetLocalPath(entryPath);
            if (File.Exists(localPath))
                return DeleteFile(localPath);
            if (Directory.Exists(localPath))
                return DeleteDirectory(localPath);
            return false;
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns></returns>
        private static bool DeleteFile(string localPath)
        {
            try
            {
                File.Delete(localPath);
                return true;
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return false;
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns></returns>
        private static bool DeleteDirectory(string localPath)
        {
            try
            {
                Directory.Delete(localPath);
                return true;
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return false;
        }

        /// <summary>
        /// Creates the file and returns a writable stream.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// A writable stream or null if creation fails (entry exists or path not found)
        /// </returns>
        public Stream CreateFile(OnePath filePath)
        {
            var localPath = GetLocalPath(filePath);
            if (Directory.Exists(localPath) || File.Exists(localPath))
                return null;
            if (!Directory.Exists(GetLocalPath(filePath.GetParent())))
                return null;

            return File.Create(localPath);
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>
        /// true if directory was created
        /// </returns>
        public bool CreateDirectory(OnePath directoryPath)
        {
            var localPath = GetLocalPath(directoryPath);
            if (Directory.Exists(localPath) || File.Exists(localPath))
                return false;
            if (!Directory.Exists(GetLocalPath(directoryPath.GetParent())))
                return false;

            try
            {
                Directory.CreateDirectory(localPath);
                return true;
            }
            catch (IOException)
            { }
            catch (UnauthorizedAccessException)
            { }
            return false;
        }
    }
}
