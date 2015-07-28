#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Protocols.Ftp
{
    using System.Net;
    using ArxOne.Ftp;

    /// <summary>
    /// FTPES variant of FTP protocol
    /// </summary>
    public class FtpesProtocolFilesystem : FtpProtocolFilesystem
    {
        /// <summary>
        /// Gets the protocol.
        /// If this is null, then the Handle() method has to be called to see if the file is handled here
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public override string Protocol => "ftpes";

        /// <summary>
        /// Gets the FTP protocol.
        /// </summary>
        /// <value>
        /// The FTP protocol.
        /// </value>
        protected override FtpProtocol FtpProtocol => FtpProtocol.FtpES;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpesProtocolFilesystem"/> class.
        /// </summary>
        /// <param name="credentialsByHost">The credentials by host.</param>
        /// <param name="parameters"></param>
        public FtpesProtocolFilesystem(ICredentialsByHost credentialsByHost, OneFilesystemParameters parameters)
            : base(credentialsByHost, parameters)
        {
        }
    }
}