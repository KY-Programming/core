using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests
{
    [TestClass]
    public class ObjectExtensionTests
    {
        [TestMethod]
        public void TestCopyStringProperty()
        {
            StringObject source = new StringObject { Property = "Test" };
            StringObject target = new StringObject();
            target.SetFrom(source);
            Assert.AreEqual("Test", target.Property);
        }

        [TestMethod]
        public void TestCopyIntProperty()
        {
            IntObject source = new IntObject { Property = 3 };
            IntObject target = new IntObject();
            target.SetFrom(source);
            Assert.AreEqual(3, target.Property);
        }

        [TestMethod]
        public void TestCopyListProperty()
        {
            ListObject source = new ListObject { List = new List<string> { "One", "Two" } };
            ListObject target = new ListObject();
            target.SetFrom(source);
            Assert.AreEqual("One", target.List[0]);
            Assert.AreEqual("Two", target.List[1]);
        }

        [TestMethod]
        public void TestCopyReadonlyListProperty()
        {
            ReadonlyListObject source = new ReadonlyListObject { List = { "One", "Two" } };
            ReadonlyListObject target = new ReadonlyListObject();
            target.SetFrom(source);
            Assert.AreEqual("One", target.List[0]);
            Assert.AreEqual("Two", target.List[1]);
        }

        private class IntObject
        {
            public int Property { get; set; }
        }

        private class ListObject
        {
            public List<string> List { get; set; }
        }

        private class ReadonlyListObject
        {
            public List<string> List { get; }

            public ReadonlyListObject()
            {
                this.List = new List<string>();
            }
        }

        private class StringObject
        {
            public string Property { get; set; }
        }
    }
}