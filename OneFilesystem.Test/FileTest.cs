#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Test
{
    using System;
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
            var entryInformations = oneFilesystem.EnumerateEntries("file://localhost");
            Assert.IsTrue(entryInformations.Any(e => e.Name == "C:"));
        }
        [TestMethod]
        [TestCategory("File")]
        [TestCategory("Local")]
        public void EnumerateDirectoryTest()
        {
            var oneFilesystem = new OneFilesystem();
            var entryInformations = oneFilesystem.EnumerateEntries("file://localhost/C:");
            Assert.IsTrue(entryInformations.Any(e => string.Equals(e.Name, "Windows", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}