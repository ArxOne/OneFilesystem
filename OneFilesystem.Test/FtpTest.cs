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
    using IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FtpTest
    {
        [TestMethod]
        [TestCategory("File")]
        [TestCategory("Local")]
        public void FromFileLocalUriTest()
        {
            using (var oneFilesystem = new OneFilesystem())
            {
                //var entryInformations = oneFilesystem.EnumerateEntries("ftp://secureftp-test.com/").ToArray();
                var entryInformations = oneFilesystem.EnumerateEntries("ftp://mirrors.kernel.org/debian-cd/").ToArray();
                var anyFile = entryInformations.FirstOrDefault(f => !f.IsDirectory && Path.GetExtension(f.Name) != "");
                using (var s = oneFilesystem.OpenRead(anyFile))
                {
                    var b = new Byte[anyFile.Length.Value];
                    s.ReadAll(b, 0, b.Length);
                    Assert.AreEqual(-1, s.ReadByte());
                }
            }
        }
    }
}
