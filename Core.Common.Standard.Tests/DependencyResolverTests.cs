using System.Collections.Generic;
using KY.Core.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests
{
    [TestClass]
    public class DependencyResolverTests
    {
        private readonly DependencyResolver resolver = new DependencyResolver();

        [TestMethod]
        public void TestResolveOfList()
        {
            this.resolver.Bind<List<File>>().To(new List<File>());
            List<File> files = this.resolver.Get<List<File>>();
            files.Add(new File("eins.test"));
            List<File> files2 = this.resolver.Get<List<File>>();
            Assert.AreEqual(files.Count, files2.Count, "file count does not match");
            Assert.AreEqual(files[0], files2[0], "file instance does not match");
        }

        private class File
        {
            public string Name { get; }

            public File(string name)
            {
                this.Name = name;
            }
        }
    }
}
