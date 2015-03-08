
namespace ArxOne.OneFilesystem.Protocols
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;

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

        private static OneEntryInformation CreateEntryInformation(OnePath path, string localPath, bool isDirectory)
        {
            if (isDirectory)
            {
                var directoryInfo = new DirectoryInfo(localPath);
                return new OneEntryInformation(path, true, null, directoryInfo.CreationTimeUtc, directoryInfo.LastWriteTimeUtc, directoryInfo.LastAccessTimeUtc);
            }

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

        private IEnumerable<OneEntryInformation> EnumerateServerEntries(OnePath directoryPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enumerates the entries using the defaut filesystem.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns></returns>
        private IEnumerable<OneEntryInformation> EnumerateDefaultEntries(OnePath directoryPath)
        {
            var localPath = GetLocalPath(directoryPath);
            if (File.Exists(localPath))
                return new OneEntryInformation[0];
            if (Directory.Exists(localPath))
            {
                return Directory.EnumerateDirectories(localPath).Select(n => CreateEntryInformation(directoryPath + n, Path.Combine(localPath, n), true))
                    .Union(Directory.EnumerateFiles(localPath).Select(n => CreateEntryInformation(directoryPath + n, Path.Combine(localPath, n), false)));
            }
            return null;
        }

        public OneEntryInformation GetInformation(OnePath entryPath)
        {
            throw new System.NotImplementedException();
        }

        public Stream OpenRead(OnePath filePath)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(OnePath entryPath)
        {
            throw new System.NotImplementedException();
        }

        public Stream CreateFile(OnePath filePath)
        {
            throw new System.NotImplementedException();
        }

        public void CreateDirectory(OnePath directoryPath)
        {
            throw new System.NotImplementedException();
        }
    }
}
