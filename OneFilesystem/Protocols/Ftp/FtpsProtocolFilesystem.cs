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

    public class FtpsProtocolFilesystem : FtpProtocolFilesystem
    {
        public override string Protocol
        {
            get { return "ftps"; }
        }

        protected override FtpProtocol FtpProtocol
        {
            get { return FtpProtocol.FtpS; }
        }

        public FtpsProtocolFilesystem(ICredentialsByHost credentialsByHost, OneFilesystemParameters parameters)
            : base(credentialsByHost, parameters)
        {
        }
    }
}