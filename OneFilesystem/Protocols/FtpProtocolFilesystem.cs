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
    using System.Net.FtpClient;
    using IO;
    using Session;

    /// <summary>
    /// FTP protocol filesystem.
    /// Uses System.Net.FtpClient
    /// </summary>
    public class FtpProtocolFilesystem : IOneProtocolFilesystem
    {
        private readonly ICredentialsByHost _credentialsByHost;
        private readonly SessionProvider<FtpClient> _sessionProvider = new SessionProvider<FtpClient>(c => c.IsConnected && !c.IsDisposed);

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
            _sessionProvider.Dispose();
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
        /// <param name="ftpClient">The FTP client.</param>
        /// <param name="entryPath">The entry path.</param>
        /// <param name="ftpListItem">The list item.</param>
        /// <returns></returns>
        private static OneEntryInformation CreateEntryInformation(FtpClient ftpClient, OnePath entryPath, FtpListItem ftpListItem)
        {
            if (ftpListItem.Type == FtpFileSystemObjectType.Link)
            {
                ftpListItem = ftpClient.DereferenceLink(ftpListItem);
                entryPath = entryPath.GetRoot() + ftpListItem.FullName;
            }
            bool isDirectory = ftpListItem.Type == FtpFileSystemObjectType.Directory;
            return new OneEntryInformation(entryPath, isDirectory, isDirectory ? (long?)null : ftpListItem.Size,
                ftpListItem.Created, ftpListItem.Modified);
        }

        /// <summary>
        /// Gets the client session.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private ClientSession<FtpClient> GetClientSession(OnePath path)
        {
            var networkCredential = GetNetworkCredential(path);
            return _sessionProvider.Get(path.Host, path.Port ?? 0, networkCredential.UserName,
                () => CreateClientSession(path.Host, path.Port, networkCredential));
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
        /// Creates the client session.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="networkCredential">The network credential.</param>
        /// <returns></returns>
        protected virtual FtpClient CreateClientSession(string host, int? port, NetworkCredential networkCredential)
        {
            var ftpClient = new FtpClient();
            ftpClient.Host = host;
            ftpClient.Port = port ?? DefaultPort;
            ftpClient.Credentials = networkCredential;
            ftpClient.ValidateCertificate += delegate(FtpClient client, FtpSslValidationEventArgs validationEventArgs)
                                                { validationEventArgs.Accept = true; };
            ftpClient.Connect();
            return ftpClient;
        }

        /// <summary>
        /// Enumerates the entries.
        /// </summary>
        /// <param name="directoryPath">A directory path to get listing from</param>
        /// <returns>
        /// A list, or null if the directory is not found (if the directoryPath points to a file, an empty list is returned)
        /// </returns>
        public IEnumerable<OneEntryInformation> EnumerateEntries(OnePath directoryPath)
        {
            using (var clientSession = GetClientSession(directoryPath))
            {
                var ftpClient = clientSession.Session;
                return ftpClient.GetListing(GetLocalPath(directoryPath)).Select(e => CreateEntryInformation(ftpClient, directoryPath + e.Name, e))
                    .Where(i => i != null).ToList();
            }
        }

        private readonly HashSet<Tuple<string, int?>> _avoidGetObjectInfo = new HashSet<Tuple<string, int?>>();

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
            using (var clientSession = GetClientSession(entryPath))
            {
                FtpListItem ftpListItem = null;
                var t = Tuple.Create(entryPath.Host, entryPath.Port);
                if (!_avoidGetObjectInfo.Contains(t) && clientSession.Session.Capabilities.HasFlag(FtpCapability.MLSD))
                {
                    try
                    {
                        ftpListItem = clientSession.Session.GetObjectInfo(GetLocalPath(entryPath));
                    }
                    catch (NullReferenceException)
                    {
                        _avoidGetObjectInfo.Add(t);
                    }
                }
                if (ftpListItem == null)
                    ftpListItem = clientSession.Session.GetListing(GetLocalPath(entryPath.GetParent())).SingleOrDefault(f => f.Name == entryPath.Name);
                return CreateEntryInformation(clientSession.Session, entryPath, ftpListItem);
            }
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
            var clientSession = GetClientSession(filePath);
            try
            {
                var ftpStream = clientSession.Session.OpenRead(GetLocalPath(filePath), FtpDataType.Binary);
                var stream = new VirtualStream(ftpStream);
                stream.Disposed += delegate { clientSession.Dispose(); };
                return stream;
            }
            catch (FtpException)
            {
            }
            clientSession.Dispose();
            return null;
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
            using (var clientSession = GetClientSession(entryPath))
            {
                try
                {
                    var localPath = GetLocalPath(entryInformation);
                    if (entryInformation.IsDirectory)
                        clientSession.Session.DeleteDirectory(localPath);
                    else
                        clientSession.Session.DeleteFile(localPath);
                    return true;
                }
                catch (FtpException)
                { }
                return false;
            }
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
            var clientSession = GetClientSession(filePath);
            try
            {
                var ftpStream = clientSession.Session.OpenWrite(GetLocalPath(filePath), FtpDataType.Binary);
                var stream = new VirtualStream(ftpStream);
                stream.Disposed += delegate { clientSession.Dispose(); };
                return stream;
            }
            catch (FtpException)
            {
            }
            clientSession.Dispose();
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
            using (var clientSession = GetClientSession(directoryPath))
            {
                try
                {
                    clientSession.Session.CreateDirectory(GetLocalPath(directoryPath));
                    return true;
                }
                catch (FtpException)
                { }
                return false;
            }
        }
    }
}
