using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests
{
    [TestClass]
    public class AssemblyLoadTests
    {
        [TestMethod]
        public void LoadAnAlphaVersion()
        {
            NugetAssemblyLocator locator = NugetPackageDependencyLoader.CreateLocator();
            // Assembly assembly = locator.Locate("Polly, Version=6.0.0.0, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc");
            Assembly assembly = locator.Locate("amotiq.base.api, Version=0.0.1.1, Culture=neutral, PublicKeyToken=c8a3ffc3f8f825cc");
            Assert.IsNotNull(assembly);
        }
    }
}
