#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FileTest
    {
        [TestMethod]
        [TestCategory("File")]
        [TestCategory("Local")]
        public void FromFileLocalUriTest()
        {
            var oneFilesystem = new OneFilesystem();
            var entryInformations = oneFilesystem.GetChildren("file://localhost");
            Assert.IsTrue(entryInformations.Any(e => e.Name == "C:"));
        }
        [TestMethod]
        [TestCategory("File")]
        [TestCategory("Local")]
        public void EnumerateDirectoryTest()
        {
            var oneFilesystem = new OneFilesystem();
            var entryInformations = oneFilesystem.GetChildren("file://localhost/C:");
            Assert.IsTrue(entryInformations.Any(e => string.Equals(e.Name, "Windows", StringComparison.InvariantCultureIgnoreCase)));
        }
        [TestMethod]
        [TestCategory("File")]
        [TestCategory("Local")]
        public void CreateFileTest()
        {
            var oneFilesystem = new OneFilesystem();
            var filePath = new OnePath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            try
            {
                using (var s = oneFilesystem.CreateFile(filePath))
                    s.WriteByte(1);

                using (var s2 = oneFilesystem.OpenRead(filePath))
                {
                    Assert.AreEqual(1, s2.ReadByte());
                    Assert.AreEqual(-1, s2.ReadByte());
                }
            }
            finally
            {
                oneFilesystem.Delete(filePath);
            }
        }

        [TestMethod]
        [TestCategory("File")]
        [TestCategory("Local")]
        public void GetFileServersTest()
        {
            using (var oneFilesystem = new OneFilesystem())
            {
                var entryInformations = oneFilesystem.GetChildren("file://").ToList();
            }
        }

        [TestMethod]
        [TestCategory("File")]
        [TestCategory("Local")]
        public void GetLocalhostTest()
        {
            using (var oneFilesystem = new OneFilesystem())
            {
                var firstEntry = oneFilesystem.GetChildren("file://").First();
                Assert.AreEqual("localhost", firstEntry.Host);
            }
        }

        [TestMethod]
        [TestCategory("File")]
        [TestCategory("Local")]
        public void GetFileSharesTest()
        {
            using (var oneFilesystem = new OneFilesystem())
            {
                var aServer = oneFilesystem.GetChildren("file://").FirstOrDefault();
                if (aServer == null)
                    Assert.Inconclusive("No file server found here");
                var shares = oneFilesystem.GetChildren(aServer).ToList();
            }
        }
    }
}
