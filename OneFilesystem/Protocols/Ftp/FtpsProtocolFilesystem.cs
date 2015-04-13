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
    /// FTPS variant of FTP protocol
    /// </summary>
    public class FtpsProtocolFilesystem : FtpProtocolFilesystem
    {
        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public override string Protocol
        {
            get { return "ftps"; }
        }

        /// <summary>
        /// Gets the FTP protocol.
        /// </summary>
        /// <value>
        /// The FTP protocol.
        /// </value>
        protected override FtpProtocol FtpProtocol
        {
            get { return FtpProtocol.FtpS; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpsProtocolFilesystem"/> class.
        /// </summary>
        /// <param name="credentialsByHost">The credentials by host.</param>
        /// <param name="parameters"></param>
        public FtpsProtocolFilesystem(ICredentialsByHost credentialsByHost, OneFilesystemParameters parameters)
            : base(credentialsByHost, parameters)
        {
        }
    }
}