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

    public class FtpesProtocolFilesystem : FtpProtocolFilesystem
    {
        public override string Protocol
        {
            get { return "ftpes"; }
        }

        protected override FtpProtocol FtpProtocol
        {
            get { return FtpProtocol.FtpES; }
        }

        public FtpesProtocolFilesystem(ICredentialsByHost credentialsByHost)
            : base(credentialsByHost)
        {
        }
    }
}