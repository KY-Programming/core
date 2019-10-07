using System.Xml;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XNodeExtension
    {
        public static string InnerXml(this XNode element)
        {
            XmlReader reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        public static string OuterXml(this XNode element)
        {
            XmlReader reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadOuterXml();
        }
    }
}