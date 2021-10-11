using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests
{
    [TestClass]
    public class SemanticVersionTests
    {
        [TestMethod]
        public void Given_Two_Equal_Semantic_Version_Then_Return_True()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            SemanticVersion two = new SemanticVersion(1, 2, 3, 4);
            Assert.IsTrue(one.Equals(two));
        }

        [TestMethod]
        public void Given_A_Semantic_Version_And_A_Equal_System_Version_Then_Return_True()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            Version two = new Version(1, 2, 3, 4);
            Assert.IsTrue(one.Equals(two));
        }

        [TestMethod]
        public void Given_Two_Major_Different_Semantic_Version_Then_Return_False()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            SemanticVersion two = new SemanticVersion(2, 2, 3, 4);
            Assert.IsFalse(one.Equals(two));
        }

        [TestMethod]
        public void Given_A_Semantic_Version_And_A_Major_Different_System_Version_Then_Return_False()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            Version two = new Version(2, 2, 3, 4);
            Assert.IsFalse(one.Equals(two));
        }

        [TestMethod]
        public void Given_Two_Minor_Different_Semantic_Version_Then_Return_False()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            SemanticVersion two = new SemanticVersion(1, 1, 3, 4);
            Assert.IsFalse(one.Equals(two));
        }

        [TestMethod]
        public void Given_A_Semantic_Version_And_A_Minor_Different_System_Version_Then_Return_False()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            Version two = new Version(1, 1, 3, 4);
            Assert.IsFalse(one.Equals(two));
        }

        [TestMethod]
        public void Given_Two_Build_Different_Semantic_Version_Then_Return_False()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            SemanticVersion two = new SemanticVersion(1, 2, 2, 4);
            Assert.IsFalse(one.Equals(two));
        }

        [TestMethod]
        public void Given_A_Semantic_Version_And_A_Build_Different_System_Version_Then_Return_False()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            Version two = new Version(1, 2, 2, 4);
            Assert.IsFalse(one.Equals(two));
        }

        [TestMethod]
        public void Given_Two_Revision_Different_Semantic_Version_Then_Return_False()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            SemanticVersion two = new SemanticVersion(1, 2, 3, 3);
            Assert.IsFalse(one.Equals(two));
        }

        [TestMethod]
        public void Given_A_Semantic_Version_And_A_Revision_Different_System_Version_Then_Return_False()
        {
            SemanticVersion one = new SemanticVersion(1, 2, 3, 4);
            Version two = new Version(1, 2, 3, 3);
            Assert.IsFalse(one.Equals(two));
        }

        [TestMethod]
        public void Given_Two_Equal_Semantic_Pre_Version_Then_Return_True()
        {
            SemanticVersion one = new SemanticVersion("1.2.3-alpha.4");
            SemanticVersion two = new SemanticVersion("1.2.3-alpha.4");
            Assert.IsTrue(one.Equals(two));
        }

        [TestMethod]
        public void Sort_By_Major()
        {
            SemanticVersion first = new SemanticVersion("1.0.0.0");
            SemanticVersion second = new SemanticVersion("2.0.0.0");
            SemanticVersion third = new SemanticVersion("11.0.0.0");
            List<SemanticVersion> list = new List<SemanticVersion> { third, second, first };
            list.Sort();
            Assert.AreEqual(first, list[0]);
            Assert.AreEqual(second, list[1]);
            Assert.AreEqual(third, list[2]);
        }

        [TestMethod]
        public void Sort_By_Minor()
        {
            SemanticVersion first = new SemanticVersion("1.0.0.0");
            SemanticVersion second = new SemanticVersion("1.1.0.0");
            SemanticVersion third = new SemanticVersion("1.11.0.0");
            List<SemanticVersion> list = new List<SemanticVersion> { third, second, first };
            list.Sort();
            Assert.AreEqual(first, list[0]);
            Assert.AreEqual(second, list[1]);
            Assert.AreEqual(third, list[2]);
        }

        [TestMethod]
        public void Sort_By_Build()
        {
            SemanticVersion first = new SemanticVersion("1.0.0.0");
            SemanticVersion second = new SemanticVersion("1.0.1.0");
            SemanticVersion third = new SemanticVersion("1.0.11.0");
            List<SemanticVersion> list = new List<SemanticVersion> { third, second, first };
            list.Sort();
            Assert.AreEqual(first, list[0]);
            Assert.AreEqual(second, list[1]);
            Assert.AreEqual(third, list[2]);
        }

        [TestMethod]
        public void Sort_By_Revision()
        {
            SemanticVersion first = new SemanticVersion("1.0.0.0");
            SemanticVersion second = new SemanticVersion("1.0.0.1");
            SemanticVersion third = new SemanticVersion("1.0.0.11");
            List<SemanticVersion> list = new List<SemanticVersion> { third, second, first };
            list.Sort();
            Assert.AreEqual(first, list[0]);
            Assert.AreEqual(second, list[1]);
            Assert.AreEqual(third, list[2]);
        }

        [TestMethod]
        public void Sort_With_Pre_1()
        {
            SemanticVersion first = new SemanticVersion("1.0.0-alpha.0");
            SemanticVersion second = new SemanticVersion("1.0.0-alpha.1");
            SemanticVersion third = new SemanticVersion("1.0.0.0");
            List<SemanticVersion> list = new List<SemanticVersion> { third, second, first };
            list.Sort();
            Assert.AreEqual(first, list[0]);
            Assert.AreEqual(second, list[1]);
            Assert.AreEqual(third, list[2]);
        }

        [TestMethod]
        public void Sort_With_Pre_2()
        {
            SemanticVersion first = new SemanticVersion("1.0-alpha.0.0");
            SemanticVersion second = new SemanticVersion("1.0.0-alpha.1");
            SemanticVersion third = new SemanticVersion("1.0.0.0");
            List<SemanticVersion> list = new List<SemanticVersion> { third, second, first };
            list.Sort();
            Assert.AreEqual(first, list[0]);
            Assert.AreEqual(second, list[1]);
            Assert.AreEqual(third, list[2]);
        }

        [TestMethod]
        public void Parse_Major_And_Minor_And_Build()
        {
            SemanticVersion version = new SemanticVersion("1.2.3");
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Build);
            Assert.AreEqual(-1, version.Revision);
            Assert.AreEqual(string.Empty, version.MajorPre);
            Assert.AreEqual(string.Empty, version.MinorPre);
            Assert.AreEqual(string.Empty, version.BuildPre);
            Assert.AreEqual(string.Empty, version.RevisionPre);
        }

        [TestMethod]
        public void Parse_Major_And_Minor()
        {
            SemanticVersion version = new SemanticVersion("1.2");
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(-1, version.Build);
            Assert.AreEqual(-1, version.Revision);
            Assert.AreEqual(string.Empty, version.MajorPre);
            Assert.AreEqual(string.Empty, version.MinorPre);
            Assert.AreEqual(string.Empty, version.BuildPre);
            Assert.AreEqual(string.Empty, version.RevisionPre);
        }
    }
}
