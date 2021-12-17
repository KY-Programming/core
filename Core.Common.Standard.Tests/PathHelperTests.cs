using KY.Core.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests
{
    [TestClass]
    public class PathHelperTests
    {
        [TestMethod]
        public void TestCombineWithRelativePathAndDuplicateDirectoryName()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.Combine(@"C:\One\Two\Three\Two", "../Four");
            Assert.AreEqual(@"C:\One\Two\Three\Four", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoRelativePaths()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo("./", "./one");
            Assert.AreEqual(@"..", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoRelativePaths1()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo("./", "./one/two");
            Assert.AreEqual(@"..\..", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoRelativePaths2()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo("./one", "./");
            Assert.AreEqual(@"one", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoRelativePaths3()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo("./one/two", "./");
            Assert.AreEqual(@"one\two", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoRelativePaths4()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo("./one", "./two");
            Assert.AreEqual(@"..\one", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoRelativePaths5()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo("./relative/one", "./relative/two");
            Assert.AreEqual(@"..\one", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoRelativePaths6()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo("./relativeA/one", "./relativeB/two");
            Assert.AreEqual(@"..\..\relativeA\one", result);
        }

        [TestMethod]
        public void TestRelativeToWithRelativeAndAbsolutePath()
        {
            PathHelper pathHelper = new(@"C:\some\Absolute\path");
            string result = pathHelper.RelativeTo("./relative", @"C:\some\Absolute\path");
            Assert.AreEqual(@"relative", result);
        }

        [TestMethod]
        public void TestRelativeToWithAbsoluteAndRelativePath()
        {
            PathHelper pathHelper = new(@"C:\some\Absolute\path");
            string result = pathHelper.RelativeTo(@"C:\some\Absolute\path", "./relative");
            Assert.AreEqual(@"..", result);
        }

        [TestMethod]
        public void TestRelativeToWithAbsoluteAndRelativePath2()
        {
            PathHelper pathHelper = new(@"C:\some\Absolute\path");
            string result = pathHelper.RelativeTo(@"C:\some\other\path", "./relative");
            Assert.AreEqual(@"..\..\..\other\path", result);
        }

        [TestMethod]
        public void TestRelativeToWithRelativeParentAndAbsolutePath()
        {
            PathHelper pathHelper = new(@"C:\some\Absolute\path");
            string result = pathHelper.RelativeTo("../relative", @"C:\some\Absolute\path");
            Assert.AreEqual(@"..\relative", result);
        }

        [TestMethod]
        public void TestRelativeToWithRelativeParentAndAbsolutePath2()
        {
            PathHelper pathHelper = new(@"C:\some\Absolute\path");
            string result = pathHelper.RelativeTo("../relative", @"C:\some\other\path");
            Assert.AreEqual(@"..\..\Absolute\relative", result);
        }

        [TestMethod]
        public void TestRelativeToWithAbsoluteAndRelativeParentPath()
        {
            PathHelper pathHelper = new(@"C:\some\Absolute\path");
            string result = pathHelper.RelativeTo(@"C:\some\Absolute\path", "../relative");
            Assert.AreEqual(@"..\path", result);
        }

        [TestMethod]
        public void TestRelativeToWithAbsoluteAndRelativeParentPath2()
        {
            PathHelper pathHelper = new(@"C:\some\Absolute\path");
            string result = pathHelper.RelativeTo(@"C:\some\other\path", "../relative");
            Assert.AreEqual(@"..\..\other\path", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoAbsolutePaths()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo(@"C:\some\Absolute\path\one", @"C:\some\Absolute\path\two");
            Assert.AreEqual(@"..\one", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoAbsolutePaths2()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo(@"C:\some\Absolute\path\one", @"C:\some\Absolute\path\two\three");
            Assert.AreEqual(@"..\..\one", result);
        }

        [TestMethod]
        public void TestRelativeToWithTwoAbsolutePaths3()
        {
            PathHelper pathHelper = new();
            string result = pathHelper.RelativeTo(@"C:\some\Absolute\path\one\three", @"C:\some\Absolute\path\two");
            Assert.AreEqual(@"..\one\three", result);
        }
    }
}
