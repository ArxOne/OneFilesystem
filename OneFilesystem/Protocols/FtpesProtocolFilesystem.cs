#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Protocols
{
    using System.Net;
    using System.Net.FtpClient;

    public class FtpesProtocolFilesystem : FtpProtocolFilesystem
    {
        public override string Protocol
        {
            get { return "ftpes"; }
        }

        public FtpesProtocolFilesystem(ICredentialsByHost credentialsByHost)
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
            ftpClient.EncryptionMode = FtpEncryptionMode.Explicit;
            ftpClient.Connect();
            return ftpClient;
        }
    }
}