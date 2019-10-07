using System;
using System.Xml;

namespace KY.Core
{
    public static class XmlDocumentExtension
    {
        public static string GetValue(this XmlDocument document, string xpath)
        {
            var node = document.SelectSingleNode(xpath);
            if (node == null)
                return null;
            return node.InnerXml;
        }

        public static Guid GetGuid(this XmlDocument document, string xpath)
        {
            return new Guid(document.GetValue(xpath));
        }
    }
}
