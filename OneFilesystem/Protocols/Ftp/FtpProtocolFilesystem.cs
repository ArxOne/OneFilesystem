#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem.Protocols.Ftp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using ArxOne.Ftp;
    using ArxOne.Ftp.Exceptions;

    /// <summary>
    /// FTP protocol filesystem.
    /// Uses System.Net.FtpClient
    /// </summary>
    public class FtpProtocolFilesystem : IOneProtocolFilesystem
    {
        private readonly ICredentialsByHost _credentialsByHost;
        private readonly IDictionary<Tuple<FtpProtocol, string, int>, FtpClient> _clients
            = new Dictionary<Tuple<FtpProtocol, string, int>, FtpClient>();

        /// <summary>
        /// Gets the protocol.
        /// If this is null, then the Handle() method has to be called to see if the file is handled here
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public virtual string Protocol
        {
            get { return "ftp"; }
        }

        protected virtual FtpProtocol FtpProtocol { get { return FtpProtocol.Ftp; } }

        protected virtual int DefaultPort { get { return 21; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpProtocolFilesystem"/> class.
        /// </summary>
        /// <param name="credentialsByHost">The credentials by host.</param>
        public FtpProtocolFilesystem(ICredentialsByHost credentialsByHost)
        {
            _credentialsByHost = credentialsByHost;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_clients)
            {
                foreach (var ftpClient in _clients.Values)
                    ftpClient.Dispose();
                _clients.Clear();
            }
        }

        private FtpClient GetFtpClient(OnePath path)
        {
            lock (_clients)
            {
                var port = path.Port ?? DefaultPort;
                var key = Tuple.Create(FtpProtocol, path.Host, port);
                FtpClient ftpClient;
                if (_clients.TryGetValue(key, out ftpClient))
                    return ftpClient;
                _clients[key] = ftpClient = new FtpClient(FtpProtocol, path.Host, port, GetNetworkCredential(path.GetRoot()));
                return ftpClient;
            }
        }

        /// <summary>
        /// Indicates if this filesystem handles the given protocol.
        /// </summary>
        /// <param name="entryPath">The entry path.</param>
        /// <returns></returns>
        public bool Handle(OnePath entryPath)
        {
            return true;
        }

        /// <summary>
        /// Gets the local path.
        /// </summary>
        /// <param name="entryPath">The entry path.</param>
        /// <returns></returns>
        private static string GetLocalPath(OnePath entryPath)
        {
            return "/" + string.Join("/", entryPath.Path);
        }

        /// <summary>
        /// Creates the entry information.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="ftpEntry">The FTP entry.</param>
        /// <returns></returns>
        private static OneEntryInformation CreateEntryInformation(OnePath parentPath, FtpEntry ftpEntry)
        {
            if (ftpEntry.Type == FtpEntryType.Link)
                return null;
            var entryPath = parentPath + ftpEntry.Name;
            return new OneEntryInformation(entryPath, ftpEntry.Type == FtpEntryType.Directory, ftpEntry.Size, lastWriteTimeUtc: ftpEntry.Date);
        }

        /// <summary>
        /// Gets the network credential.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        protected NetworkCredential GetNetworkCredential(OnePath path)
        {
            var anonymous = new NetworkCredential("anonymous", "someone@somewhere");
            if (_credentialsByHost == null)
                return anonymous;
            var networkContext = _credentialsByHost.GetCredential(path.Host, path.Port ?? DefaultPort, Protocol);
            if (string.IsNullOrEmpty(networkContext.UserName))
                return anonymous;
            return networkContext;
        }

        /// <summary>
        /// Enumerates the entries.
        /// </summary>
        /// <param name="directoryPath">A directory path to get listing from</param>
        /// <returns>
        /// A list, or null if the directory is not found (if the directoryPath points to a file, an empty list is returned)
        /// </returns>
        public IEnumerable<OneEntryInformation> GetChildren(OnePath directoryPath)
        {
            return GetFtpClient(directoryPath).StatEntries(GetLocalPath(directoryPath)).Select(entry => CreateEntryInformation(directoryPath, entry))
                .Where(i => i != null).ToList();
        }

        /// <summary>
        /// Gets the information about the referenced file.
        /// </summary>
        /// <param name="entryPath">A file path to get information about</param>
        /// <returns>
        /// Information or null if entry is not found
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public OneEntryInformation GetInformation(OnePath entryPath)
        {
            return GetChildren(entryPath.GetParent()).SingleOrDefault(n => n.Name == entryPath.Name);
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
            return new FtpStream(GetFtpClient(filePath).Retr(GetLocalPath(filePath)));
        }

        /// <summary>
        /// Deletes the specified file or directory (does not recurse directories).
        /// </summary>
        /// <param name="entryPath"></param>
        /// <returns>
        /// true is entry was successfully deleted
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Delete(OnePath entryPath)
        {
            // because we need to know whether this is a directory
            var entryInformation = entryPath as OneEntryInformation;
            if (entryInformation == null)
                return Delete(GetInformation(entryPath));
            // here, entryInformation is valid
            try
            {
                var localPath = GetLocalPath(entryInformation);
                if (entryInformation.IsDirectory)
                    GetFtpClient(entryPath).Rmd(localPath);
                else
                    GetFtpClient(entryPath).Dele(localPath);
                return true;
            }
            catch (FtpProtocolException)
            { }
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
            try
            {
                return new FtpStream(GetFtpClient(filePath).Stor(GetLocalPath(filePath)));
            }
            catch (FtpProtocolException)
            { }
            return null;
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
            try
            {
                GetFtpClient(directoryPath).Mkd(GetLocalPath(directoryPath));
                return true;
            }
            catch (FtpProtocolException)
            { }
            return false;
        }
    }
}
