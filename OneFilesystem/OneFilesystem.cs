#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using Protocols;
    using Protocols.File;
    using Protocols.Ftp;
    using Protocols.Sftp;

    /// <summary>
    /// OneFilesystem implementation
    /// </summary>
    public class OneFilesystem : IOneFilesystem, IDisposable
    {
        private readonly Dictionary<string, IList<IOneProtocolFilesystem>> _protocolFilesystemsByProtocol;
        private readonly IList<IOneProtocolFilesystem> _nullFilesystems;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneFilesystem" /> class.
        /// </summary>
        /// <param name="credentialsByHost">The credentials by host.</param>
        /// <param name="protocolFilesystems">The protocol filesystems.</param>
        /// <param name="proxy">The proxy.</param>
        public OneFilesystem(ICredentialsByHost credentialsByHost = null, IOneProtocolFilesystem[] protocolFilesystems = null, Func<EndPoint, Socket> proxy = null)
        {
            var validProtocolFilesystems = protocolFilesystems ?? CreateDefaultFilesystems(credentialsByHost, proxy);
            _protocolFilesystemsByProtocol = validProtocolFilesystems.Where(p => p.Protocol != null)
                .GroupBy(p => p.Protocol)
                .ToDictionary(p => p.Key, p => (IList<IOneProtocolFilesystem>)p.ToList());
            _nullFilesystems = validProtocolFilesystems.Where(p => p.Protocol == null).ToList();
        }

        /// <summary>
        /// Creates the default filesystems.
        /// </summary>
        /// <param name="credentialsByHost">The credentials by host.</param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        private static IOneProtocolFilesystem[] CreateDefaultFilesystems(ICredentialsByHost credentialsByHost, Func<EndPoint, Socket> proxy)
        {
            return new IOneProtocolFilesystem[]
            {
                new FileProtocolFilesystem(), new FtpProtocolFilesystem(credentialsByHost, proxy),
                new FtpesProtocolFilesystem(credentialsByHost,proxy), new FtpsProtocolFilesystem(credentialsByHost, proxy),
                new SftpProtocolFilesystem(credentialsByHost),
            };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var filesystem in _protocolFilesystemsByProtocol.Values.SelectMany(v => v))
                filesystem.Dispose();
            foreach (var filesystem in _nullFilesystems)
                filesystem.Dispose();
        }

        /// <summary>
        /// Gets the filesystem.
        /// </summary>
        /// <param name="onePath">The one path.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        private IOneProtocolFilesystem GetFilesystem(OnePath onePath)
        {
            IOneProtocolFilesystem filesystem;
            IList<IOneProtocolFilesystem> filesystems;
            if (_protocolFilesystemsByProtocol.TryGetValue(onePath.Protocol, out filesystems))
            {
                filesystem = filesystems.FirstOrDefault(f => f.Handle(onePath));
                if (filesystem != null)
                    return filesystem;
            }

            filesystem = _nullFilesystems.FirstOrDefault(f => f.Handle(onePath));
            if (filesystem != null)
                return filesystem;

            throw new NotSupportedException(string.Format("Found no filesystem to handle URI '{0}'", onePath));
        }

        /// <summary>
        /// Enumerates the entries.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>
        /// A list, or null if the directory is not found (if the directoryPath points to a file, an empty list is returned)
        /// </returns>
        public IEnumerable<OneEntryInformation> GetChildren(OnePath directoryPath)
        {
            var filesystem = GetFilesystem(directoryPath);
            return filesystem.GetChildren(directoryPath);
        }

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <param name="entryPath"></param>
        /// <returns></returns>
        public OneEntryInformation GetInformation(OnePath entryPath)
        {
            // A quick check to avoid annoying everyone
            var entryInformation = entryPath as OneEntryInformation;
            if (entryInformation != null)
                return entryInformation;
            var filesystem = GetFilesystem(entryPath);
            return filesystem.GetInformation(entryPath);
        }

        /// <summary>
        /// Opens file for reading.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// A readable stream, or null if the file does not exist or is a directory
        /// </returns>
        public Stream OpenRead(OnePath filePath)
        {
            var filesystem = GetFilesystem(filePath);
            return filesystem.OpenRead(filePath);
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
            var filesystem = GetFilesystem(entryPath);
            return filesystem.Delete(entryPath);
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
            var filesystem = GetFilesystem(filePath);
            return filesystem.CreateFile(filePath);
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
            var filesystem = GetFilesystem(directoryPath);
            return filesystem.CreateDirectory(directoryPath);
        }
    }
}
