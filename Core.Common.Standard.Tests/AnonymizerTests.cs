using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests
{
    [TestClass]
    public class AnonymizerTests
    {
        [TestMethod]
        public void Given_An_Ip_Then_Keep_The_Structure_Valid()
        {
            Anonymizer.ReplaceIps("192.168.0.1").Split('.').Select(int.Parse)
                      .ForEach(x => Assert.IsTrue(x <= 255 && x >= 0));
        }

        [TestMethod]
        public void Given_An_Ip_Then_Return_An_Other_Ip()
        {
            Assert.AreNotEqual("10.0.0.1", Anonymizer.ReplaceIps("10.0.0.1"));
            Assert.AreNotEqual("10.0.0.10", Anonymizer.ReplaceIps("10.0.0.10"));
            Assert.AreNotEqual("192.168.0.1", Anonymizer.ReplaceIps("192.168.0.1"));
            Assert.AreNotEqual("192.168.20.1", Anonymizer.ReplaceIps("192.168.20.1"));
        }

        [TestMethod]
        public void Given_An_Text_With_An_Ip_Then_Replace_Only_The_Ip()
        {
            Assert.AreNotEqual("My ip is 192.168.0.1 asdf", Anonymizer.ReplaceIps("My ip is 192.168.0.1 asdf"));
        }

        [TestMethod]
        public void Given_An_Text_With_An_Url_Then_Replace_Only_The_Url()
        {
            Assert.AreNotEqual("This is a url http://ky-programming.de/ test", Anonymizer.ReplaceUrls("This is a url http://ky-programming.de/ test"));
            Assert.AreNotEqual("This is a url https://ky-programming.de/ test", Anonymizer.ReplaceUrls("This is a url https://ky-programming.de/ test"));
            Assert.AreNotEqual("This is a url https://sub.ky-programming.de test", Anonymizer.ReplaceUrls("This is a url https://sub.ky-programming.de test"));
            Assert.AreNotEqual("This is a url https://ky-programming.de:80/sub test", Anonymizer.ReplaceUrls("This is a url https://ky-programming.de:80/sub test"));
        }

    }
}
