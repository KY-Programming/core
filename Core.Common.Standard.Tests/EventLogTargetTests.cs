using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests;

[TestClass]
public class EventLogTargetTests
{
    [TestMethod]
    public void Constructor_With_Null_Source_Throws()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new EventLogTarget(null, "Application"));
    }

    [TestMethod]
    public void Constructor_With_Null_Name_Throws()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new EventLogTarget("KY.Core.Tests", null));
    }
}
