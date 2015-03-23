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
    }
}