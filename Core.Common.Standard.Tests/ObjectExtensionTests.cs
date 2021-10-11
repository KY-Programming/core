using System;
using System.Collections.Generic;
using KY.Core.Clone;
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

        [TestMethod]
        public void TestCloneSelfReferencingObject()
        {
            SelfReferencing source = new();
            source.Parent = source;
            SelfReferencing clone = source.Clone();
            Assert.AreEqual(clone, clone.Parent);
        }

        [TestMethod]
        public void TestCloneCircularReferencingObject()
        {
            SelfReferencing first = new() { Id = "First" };
            SelfReferencing second = new() { Id = "Second" };
            first.Parent = second;
            second.Parent = first;
            SelfReferencing clone = first.Clone();
            Assert.AreEqual(first.Id, clone.Id);
            Assert.AreEqual(second.Id, clone.Parent.Id);
            Assert.AreEqual(first.Id, clone.Parent.Parent.Id);
        }

        [TestMethod]
        public void TestCloneTwoCircularReferencingObject()
        {
            SelfReferencing first = new() { Id = "First" };
            SelfReferencing second = new() { Id = "Second" };
            SelfReferencing third = new() { Id = "Third" };
            first.Parent = second;
            second.Parent = third;
            third.Parent = first;
            SelfReferencing clone = first.Clone();
            Assert.AreEqual(first.Id, clone.Id);
            Assert.AreEqual(second.Id, clone.Parent.Id);
            Assert.AreEqual(third.Id, clone.Parent.Parent.Id);
            Assert.AreEqual(first.Id, clone.Parent.Parent.Parent.Id);
        }

        [TestMethod]
        public void TestCloneCircularWithICloneable()
        {
            CircularICloneable first = new() { Id = "First" };
            CircularOther second = new() { Id = "Second" };
            first.Parent = second;
            second.Parent = first;
            CircularICloneable clone = first.Clone();
            Assert.AreEqual(first.Id, clone.Id);
            Assert.AreEqual(second.Id, clone.Parent.Id);
            Assert.AreEqual(first.Id, clone.Parent.Parent.Id);
        }

        [TestMethod]
        public void TestNotCloneableProperty()
        {
            WithNotCloneableProperty source = new();
            WithNotCloneableProperty clone = source.Clone();
            Assert.AreNotEqual(source.CloneMe, clone.CloneMe);
            Assert.AreEqual(source.DoNotCloneMe, clone.DoNotCloneMe);
        }

        [TestMethod]
        public void TestTwiceTheSameProperty()
        {
            TwiceTheSameProperty source = new();
            TwiceTheSameProperty clone = source.Clone();
            Assert.AreEqual(clone.First, clone.Second);
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

        private class SelfReferencing
        {
            public string Id { get; set; }
            public SelfReferencing Parent { get; set; }
        }

        private class StringObject
        {
            public string Property { get; set; }
        }

        private class CircularICloneable : ICloneable
        {
            public string Id { get; set; }
            public CircularOther Parent { get; set; }

            object ICloneable.Clone()
            {
                CircularICloneable clone = new();
                clone.Id = this.Id;
                clone.Parent = this.Parent.Clone();
                return clone;
            }
        }

        private class CircularOther
        {
            public string Id { get; set; }
            public CircularICloneable Parent { get; set; }
        }

        private class WithNotCloneableProperty
        {
            public StringObject CloneMe { get; set; } = new();

            [NotCloneable]
            public StringObject DoNotCloneMe { get; set; } = new();
        }

        private class TwiceTheSameProperty
        {
            public StringObject First { get; set; }
            public StringObject Second { get; set; }

            public TwiceTheSameProperty()
            {
                this.First = this.Second = new StringObject();
            }
        }
    }
}
