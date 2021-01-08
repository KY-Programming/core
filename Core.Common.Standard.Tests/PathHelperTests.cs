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
            PathHelper pathHelper = new PathHelper();
            string result = pathHelper.Combine(@"C:\One\Two\Three\Two", "../Four");
            Assert.AreEqual(@"C:\One\Two\Three\Four", result);
        }
        
    }
}