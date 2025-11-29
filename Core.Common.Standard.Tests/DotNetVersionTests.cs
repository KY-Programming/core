using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests;

[TestClass]
public class DotNetVersionTests
{
    [TestMethod]
    public void Given_A_Version_Then_Return_Parsed_Version()
    {
        Assert.IsTrue(DotNetVersion.FromDirectoryName("net20") == new DotNetVersion(FrameworkType.DotNetFramework, 2), "net20 does not match");
        Assert.IsTrue(DotNetVersion.FromDirectoryName("net35") == new DotNetVersion(FrameworkType.DotNetFramework, 3, 5), "net35 does not match");
        Assert.IsTrue(DotNetVersion.FromDirectoryName("net462") == new DotNetVersion(FrameworkType.DotNetFramework, 4, 6, 2), "net462 does not match");
        Assert.IsTrue(DotNetVersion.FromDirectoryName("netstandard1.3") == new DotNetVersion(FrameworkType.DotNetStandard, 1, 3), "netstandard1.3 does not match");
        Assert.IsTrue(DotNetVersion.FromDirectoryName("netstandard2.0") == new DotNetVersion(FrameworkType.DotNetStandard, 2), "netstandard2.0 does not match");
        Assert.IsTrue(DotNetVersion.FromDirectoryName("net8.0") == new DotNetVersion(FrameworkType.DotNet, 8), "net8.0 does not match");
        Assert.IsTrue(DotNetVersion.FromDirectoryName("net9.0") == new DotNetVersion(FrameworkType.DotNet, 9), "net9.0 does not match");
    }
    
    [TestMethod]
    public void Given_A_Version_Then_Return_Same_Version_For_Same_String()
    {
        Assert.AreEqual("net20", DotNetVersion.FromDirectoryName("net20")?.ToString());
        Assert.AreEqual("net35", DotNetVersion.FromDirectoryName("net35")?.ToString());
        Assert.AreEqual("net462", DotNetVersion.FromDirectoryName("net462")?.ToString());
        Assert.AreEqual("netstandard1.3", DotNetVersion.FromDirectoryName("netstandard1.3")?.ToString());
        Assert.AreEqual("netstandard2.0", DotNetVersion.FromDirectoryName("netstandard2.0")?.ToString());
        Assert.AreEqual("net8.0", DotNetVersion.FromDirectoryName("net8.0")?.ToString());
        Assert.AreEqual("net9.0", DotNetVersion.FromDirectoryName("net9.0")?.ToString());
    }
}
