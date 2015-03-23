namespace ArxOne.OneFilesystem.Protocols
{
    using System.Net;
    using Ftp;

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