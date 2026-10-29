using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KY.Core.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests
{
    [TestClass]
    public class FileHelperTests
    {
        private string directory;

        [TestInitialize]
        public void Initialize()
        {
            this.directory = Path.Combine(Path.GetTempPath(), $"FileHelperTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(this.directory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(this.directory, true);
        }

        [TestMethod]
        public void TestWriteAllTextReplacesLongerContentCompletely()
        {
            string path = Path.Combine(this.directory, "settings.json");
            FileSystem.WriteAllText(path, "{\"value\":\"" + new string('x', 1000) + "\"}");
            FileSystem.WriteAllText(path, "{}");
            Assert.AreEqual("{}", FileSystem.ReadAllText(path));
            CollectionAssert.AreEqual(new[] { path }, Directory.GetFiles(this.directory));
        }

        [TestMethod]
        public void TestWriteAllTextWritesUtf8WithoutByteOrderMark()
        {
            string path = Path.Combine(this.directory, "text.txt");
            FileSystem.WriteAllText(path, "ä");
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes("ä"), File.ReadAllBytes(path));
        }

        [TestMethod]
        public void TestWriteAllTextWritesByteOrderMarkOfExplicitEncoding()
        {
            string path = Path.Combine(this.directory, "text.txt");
            FileSystem.WriteAllText(path, "ä", Encoding.UTF8);
            CollectionAssert.AreEqual(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes("ä")).ToArray(), File.ReadAllBytes(path));
        }

        [TestMethod]
        public void TestWriteAllBytes()
        {
            string path = Path.Combine(this.directory, "data.bin");
            FileSystem.WriteAllBytes(path, [1, 2, 3, 4]);
            FileSystem.WriteAllBytes(path, [5]);
            CollectionAssert.AreEqual(new byte[] { 5 }, File.ReadAllBytes(path));
        }

        [TestMethod]
        public void TestConcurrentWritesLeaveOneCompleteContent()
        {
            string path = Path.Combine(this.directory, "concurrent.json");
            string[] contents = Enumerable.Range(1, 50).Select(index => "{\"value\":\"" + new string('x', index * 37) + "\"}").ToArray();
            Parallel.ForEach(contents, content => FileSystem.WriteAllText(path, content));
            CollectionAssert.Contains(contents, FileSystem.ReadAllText(path));
            CollectionAssert.AreEqual(new[] { path }, Directory.GetFiles(this.directory));
        }

        [TestMethod]
        public void TestFailedWriteKeepsOldContentAndLeavesNoTemporaryFile()
        {
            string path = Path.Combine(this.directory, "settings.json");
            FileSystem.WriteAllText(path, "old");
            Encoding strictAscii = Encoding.GetEncoding("us-ascii", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            Assert.ThrowsExactly<EncoderFallbackException>(() => FileSystem.WriteAllText(path, "ä", strictAscii));
            Assert.AreEqual("old", FileSystem.ReadAllText(path));
            CollectionAssert.AreEqual(new[] { path }, Directory.GetFiles(this.directory));
        }

        [TestMethod]
        public void TestMoveFileOverwritesTarget()
        {
            string from = Path.Combine(this.directory, "from.txt");
            string to = Path.Combine(this.directory, "to.txt");
            File.WriteAllText(from, "new");
            File.WriteAllText(to, "old");
            FileSystem.MoveFile(from, to, true);
            Assert.AreEqual("new", File.ReadAllText(to));
            Assert.IsFalse(File.Exists(from));
        }
    }
}
