
namespace ArxOne.OneFilesystem.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BasicTest
    {
        private IEnumerable<Tuple<OnePath, NetworkCredential>> EnumerateCredentials()
        {
            const string credentialsTxt = "credentials.txt";
            if (!File.Exists(credentialsTxt))
                Assert.Inconclusive("File '{0}' not found", credentialsTxt);
            using (var streamReader = File.OpenText(credentialsTxt))
            {
                for (; ; )
                {
                    var line = streamReader.ReadLine();
                    if (line == null)
                        yield break;

                    var uri = new Uri(line);
                    var l = HttpUtility.UrlDecode(uri.UserInfo.Replace("_at_", "@"));
                    NetworkCredential networkCredential;
                    if (string.IsNullOrEmpty(l))
                    {
                        networkCredential = new NetworkCredential("anonymous", "here@there.com");
                    }
                    else
                    {
                        var up = l.Split(new[] { ':' }, 2);
                        networkCredential = new NetworkCredential(up[0], up[1]);
                    }
                    yield return Tuple.Create(new OnePath(uri), networkCredential);
                }
            }
        }

        private Tuple<OnePath, NetworkCredential> GetTestCredential(string protocol)
        {
            var t = EnumerateCredentials().FirstOrDefault(c => c.Item1.Protocol == protocol);
            if (t == null)
                Assert.Inconclusive("Found no configuration for protocol '{0}'", protocol);
            return t;
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftp")]
        public void FtpEnumerateFilesTest()
        {
            EnumerateFilesTest("ftp");
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftpes")]
        public void FtpesEnumerateFilesTest()
        {
            EnumerateFilesTest("ftpes");
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Sftp")]
        public void SftpEnumerateFilesTest()
        {
            EnumerateFilesTest("sftp");
        }

        private void EnumerateFilesTest(string protocol)
        {
            var t = GetTestCredential(protocol);
            using (var fs = new OneFilesystem(t.Item2))
            {
                var l = fs.GetChildren(t.Item1).ToArray();
            }
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftp")]
        public void FtpCreateDirectoryTest()
        {
            CreateDirectoryTest("ftp");
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftpes")]
        public void FtpesCreateDirectoryTest()
        {
            CreateDirectoryTest("ftpes");
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Sftp")]
        public void SftpCreateDirectoryTest()
        {
            CreateDirectoryTest("sftp");
        }

        private void CreateDirectoryTest(string protocol)
        {
            var t = GetTestCredential(protocol);
            using (var fs = new OneFilesystem(t.Item2))
            {
                var folderPath = t.Item1 + ("folder-" + Guid.NewGuid().ToString());
                try
                {
                    Assert.IsTrue(fs.CreateDirectory(folderPath));
                }
                finally
                {
                    fs.Delete(folderPath);
                }
            }
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftp")]
        public void FtpCreateFileTest()
        {
            CreateFileTest("ftp");
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftpes")]
        public void FtpesCreateFileTest()
        {
            CreateFileTest("ftpes");
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Sftp")]
        public void SftpCreateFileTest()
        {
            CreateFileTest("sftp");
        }

        private void CreateFileTest(string protocol)
        {
            var t = GetTestCredential(protocol);
            using (var fs = new OneFilesystem(t.Item2))
            {
                var filePath = t.Item1 + ("file-" + Guid.NewGuid().ToString());
                try
                {
                    using (var w = fs.CreateFile(filePath))
                        w.WriteByte(1);
                    using (var r = fs.OpenRead(filePath))
                    {
                        Assert.AreEqual(1, r.ReadByte());
                        Assert.AreEqual(-1, r.ReadByte());
                    }
                }
                finally
                {
                    fs.Delete(filePath);
                }
            }
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftp")]
        [ExpectedException(typeof(OneFilesystemAccessDeniedException))]
        public void FtpAnonymousFindHostTest()
        {
            FindHostTest("ftp", true);
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftp")]
        public void FtpFindHostTest()
        {
            FindHostTest("ftp", false);
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Sftp")]
        [ExpectedException(typeof(OneFilesystemAccessDeniedException))]
        public void SftpAnonymousFindHostTest()
        {
            FindHostTest("sftp", true);
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Sftp")]
        public void SftpFindHostTest()
        {
            FindHostTest("sftp", false);
        }

        public void FindHostTest(string protocol, bool anonymous)
        {
            var t = GetTestCredential(protocol);
            using (var fs = new OneFilesystem(anonymous ? (ICredentialsByHost)new CredentialCache() : t.Item2))
            {
                var i = fs.GetInformation(t.Item1.GetRoot());
                Assert.IsNotNull(i);
            }
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftp")]
        public void FtpFileNotFoundTest()
        {
            FileNotFoundTest("ftp");
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Sftp")]
        public void SftpFileNotFoundTest()
        {
            FileNotFoundTest("sftp");
        }

        public void FileNotFoundTest(string protocol)
        {
            var t = GetTestCredential(protocol);
            using (var fs = new OneFilesystem(t.Item2))
            {
                var i = fs.GetInformation(t.Item1.GetRoot() + "thisFileProbablyDoesNotExist!");
                Assert.IsNull(i);
            }
        }
        
        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Ftp")]
        public void FtpHostNotFoundTest()
        {
            HostNotFoundTest("ftp", "thisHostProbablyDoesNotExist");
        }

        [TestMethod]
        [DeploymentItem("credentials.txt")]
        [TestCategory("File")]
        [TestCategory("Sftp")]
        public void SftpHostNotFoundTest()
        {
            HostNotFoundTest("sftp", "thisHostProbablyDoesNotExist");
        }

        public void HostNotFoundTest(string protocol, string hostName)
        {
            var t = GetTestCredential(protocol);
            using (var fs = new OneFilesystem(new CredentialCache()))
            {
                var i = fs.GetInformation(protocol + "://" + hostName);
                Assert.IsNull(i);
            }
        }

    }
}
