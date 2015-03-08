namespace ArxOne.OneFilesystem.Protocols
{
    using System.Net;
    using System.Net.FtpClient;

    public class FtpsProtocolFilesystem : FtpProtocolFilesystem
    {
        public override string Protocol
        {
            get { return "ftps"; }
        }

        public FtpsProtocolFilesystem(ICredentialsByHost credentialsByHost)
            : base(credentialsByHost)
        {
        }

        protected override FtpClient CreateClientSession(string host, int? port, NetworkCredential networkCredential)
        {
            var ftpClient = new FtpClient();
            ftpClient.Host = host;
            ftpClient.Port = port ?? DefaultPort;
            ftpClient.Credentials = networkCredential;
            ftpClient.ValidateCertificate += delegate(FtpClient client, FtpSslValidationEventArgs validationEventArgs) { validationEventArgs.Accept = true; };
            ftpClient.EncryptionMode = FtpEncryptionMode.Implicit;
            ftpClient.Connect();
            return ftpClient;
        }
    }
}