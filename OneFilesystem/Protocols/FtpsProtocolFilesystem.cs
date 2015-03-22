namespace ArxOne.OneFilesystem.Protocols
{
    using System.Net;
    using Ftp;

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

        public FtpsProtocolFilesystem(ICredentialsByHost credentialsByHost)
            : base(credentialsByHost)
        {
        }
    }
}