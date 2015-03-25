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
    public class PathTest
    {
        [TestMethod]
        [TestCategory("Path")]
        public void FromFileLocalUriTest()
        {
            var p = new OnePath(new Uri(@"file://localhost/c:\windows"));
            Assert.AreEqual("file", p.Protocol);
            Assert.AreEqual("localhost", p.Host);
            Assert.AreEqual(null, p.Port);
            Assert.IsTrue(p.Path.SequenceEqual(new[] { "c:", "windows" }));
        }

        [TestMethod]
        [TestCategory("Path")]
        public void FromFileLocalTest()
        {
            var p = new OnePath(@"c:\windows");
            Assert.AreEqual("file", p.Protocol);
            Assert.AreEqual("localhost", p.Host);
            Assert.AreEqual(null, p.Port);
            Assert.IsTrue(p.Path.SequenceEqual(new[] { "c:", "windows" }));
        }

        [TestMethod]
        [TestCategory("Path")]
        public void FromFileShareTest()
        {
            var p = new OnePath(@"\\this\c$");
            Assert.AreEqual("file", p.Protocol);
            Assert.AreEqual("this", p.Host);
            Assert.AreEqual(null, p.Port);
            Assert.IsTrue(p.Path.SequenceEqual(new[] { "c$" }));
        }

        [TestMethod]
        [TestCategory("Path")]
        public void FromFileRootUriTest()
        {
            var p = new OnePath(@"file://");
            Assert.AreEqual("file", p.Protocol);
            Assert.IsNull(p.Host);
            Assert.AreEqual(null, p.Port);
            Assert.AreEqual(0, p.Path.Count);
        }

        [TestMethod]
        [TestCategory("Path")]
        public void FromFileRootUNCTest()
        {
            var p = new OnePath(@"\\");
            Assert.AreEqual("file", p.Protocol);
            Assert.IsNull(p.Host);
            Assert.AreEqual(null, p.Port);
            Assert.AreEqual(0, p.Path.Count);
        }

        [TestMethod]
        [TestCategory("Path")]
        public void AddRootAndNameTest()
        {
            var p = new OnePath(@"\\");
            p += "server";
            Assert.AreEqual("file", p.Protocol);
            Assert.AreEqual("server", p.Host);
            Assert.AreEqual(null, p.Port);
            Assert.AreEqual(0, p.Path.Count);
        }

        [TestMethod]
        [TestCategory("Path")]
        public void FromFileServerTest()
        {
            var p = new OnePath(@"\\aserver");
            Assert.AreEqual("file", p.Protocol);
            Assert.AreEqual("aserver", p.Host);
            Assert.AreEqual(null, p.Port);
            Assert.AreEqual(0, p.Path.Count);
        }
    }
}
