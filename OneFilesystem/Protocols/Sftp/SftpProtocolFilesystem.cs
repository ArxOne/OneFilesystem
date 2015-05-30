#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem.Protocols.Sftp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using Exceptions;
    using Renci.SshNet;
    using Renci.SshNet.Common;
    using Renci.SshNet.Sftp;
    using Session;

    /// <summary>
    /// SFTP protocol
    /// </summary>
    public class SftpProtocolFilesystem : IOneProtocolFilesystem
    {
        private const int DefaultPort = 22;
        private readonly ICredentialsByHost _credentialsByHost;
        private readonly SessionProvider<SftpClient> _sessionProvider = new SessionProvider<SftpClient>(c => c.IsConnected);

        /// <summary>
        /// Gets the protocol.
        /// If this is null, then the Handle() method has to be called to see if the file is handled here
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public string Protocol
        {
            get { return "sftp"; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SftpProtocolFilesystem"/> class.
        /// </summary>
        /// <param name="credentialsByHost">The credentials by host.</param>
        /// <param name="parameters"></param>
        public SftpProtocolFilesystem(ICredentialsByHost credentialsByHost, OneFilesystemParameters parameters)
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
        /// Gets the client for given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private ClientSession<SftpClient> GetClient(OnePath path)
        {
            var host = path.Host;
            var port = path.Port ?? DefaultPort;
            var networkCredential = GetNetworkCredential(path);
            try
            {
                return _sessionProvider.Get(host, port, networkCredential.UserName, () => CreateClient(host, port, networkCredential));
            }
            catch (SshAuthenticationException e)
            {
                throw new OneFilesystemAccessDeniedException("Access denied", e);
            }
        }

        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="networkCredential">The network credential.</param>
        /// <returns></returns>
        private static SftpClient CreateClient(string host, int port, NetworkCredential networkCredential)
        {
            var client = new SftpClient(host, port, networkCredential.UserName, networkCredential.Password);
            client.Connect();
            return client;
        }

        /// <summary>
        /// Gets the network credential for given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        protected NetworkCredential GetNetworkCredential(OnePath path)
        {
            var anonymous = new NetworkCredential("anonymous", "someone@somewhere");
            if (_credentialsByHost == null)
                return anonymous;
            var networkContext = _credentialsByHost.GetCredential(path.Host, path.Port ?? DefaultPort, Protocol);
            if (networkContext == null || string.IsNullOrEmpty(networkContext.UserName))
                return anonymous;
            return networkContext;
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
        /// Creates the entry.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private static OneEntryInformation CreateEntry(OnePath path, SftpFile file)
        {
            return new OneEntryInformation(path, file.IsDirectory, file.IsDirectory ? (long?)null : file.Length, null,
                file.LastWriteTimeUtc, file.LastAccessTimeUtc);
        }

        /// <summary>
        /// Enumerates the entries.
        /// </summary>
        /// <param name="directoryPath">A directory path to get listing from</param>
        /// <returns>
        /// A list, or null if the directory is not found (if the directoryPath points to a file, an empty list is returned)
        /// </returns>
        IEnumerable<OneEntryInformation> IOneFilesystem.GetChildren(OnePath directoryPath)
        {
            if (directoryPath.Host == "")
                return new OneEntryInformation[0];
            using (var client = GetClient(directoryPath))
            {
                return client.Session.ListDirectory(GetLocalPath(directoryPath))
                    .Where(file => file.Name != "." && file.Name != "..")
                    .Select(file => CreateEntry(directoryPath + file.Name, file)).ToList();
            }
        }

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <param name="entryPath">The entry path.</param>
        /// <returns></returns>
        OneEntryInformation IOneFilesystem.GetInformation(OnePath entryPath)
        {
            try
            {
                using (var client = GetClient(entryPath))
                    return CreateEntry(entryPath, client.Session.Get(GetLocalPath(entryPath)));
            }
            catch (SftpPathNotFoundException)
            { }
            catch (SocketException)
            { }
            return null;
        }

        /// <summary>
        /// Opens file for reading.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// A readable stream, or null if the file does not exist or is a directory
        /// </returns>
        Stream IOneFilesystem.OpenRead(OnePath filePath)
        {
            var client = GetClient(filePath);
            try
            {
                var stream = new SftpStream(client.Session.OpenRead(GetLocalPath(filePath)));
                stream.Disposed += delegate { client.Dispose(); };
                return stream;
            }
            catch (SshException)
            { }
            client.Dispose();
            return null;
        }

        /// <summary>
        /// Deletes the specified file or directory (does not recurse directories).
        /// </summary>
        /// <param name="entryPath"></param>
        /// <returns>
        /// true is entry was successfully deleted
        /// </returns>
        bool IOneFilesystem.Delete(OnePath entryPath)
        {
            using (var client = GetClient(entryPath))
            {
                try
                {
                    client.Session.Delete(GetLocalPath(entryPath));
                    return true;
                }
                catch (SshException)
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
        Stream IOneFilesystem.CreateFile(OnePath filePath)
        {
            var client = GetClient(filePath);
            try
            {
                var stream = new SftpStream(client.Session.Create(GetLocalPath(filePath)));
                stream.Disposed += delegate { client.Dispose(); };
                return stream;
            }
            catch (SshException)
            {
            }
            client.Dispose();
            return null;
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>
        /// true if directory was created
        /// </returns>
        bool IOneFilesystem.CreateDirectory(OnePath directoryPath)
        {
            var parentPath = directoryPath.GetParent();
            using (var client = GetClient(parentPath))
            {
                try
                {
                    client.Session.CreateDirectory(GetLocalPath(directoryPath));
                    return true;
                }
                catch (SshException)
                { }
                return false;
            }
        }

        /// <summary>
        /// Translates exceptions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        /// <exception cref="ArxOne.OneFilesystem.Exceptions.OneFilesystemTransportException"></exception>
        public static TResult Process<TResult>(Func<TResult> func)
        {
            try
            {
                return func();
            }
            catch (SshConnectionException e)
            {
                throw new OneFilesystemTransportException(null, e);
            }
            catch (SshException e)
            {
                throw new OneFilesystemProtocolException(null, e);
            }
        }
    }
}
