using System.Collections.Generic;
using System.Xml.Linq;
using KY.Core.Xml;

namespace KY.Core.DataAccess
{
    public class KeyValueReader : XmlReader<IEnumerable<KeyValuePair<string, string>>>
    {
        public KeyValueReader()
            : base(DataAccess.FileSystem.Create())
        { }

        public KeyValueReader(IFileSystem fileSystem)
            : base(fileSystem)
        { }

        public override IEnumerable<KeyValuePair<string, string>> Parse(string path, XElement rootElement)
        {
            foreach (XElement element in rootElement.Elements())
            {
                string key = element.Name.ToString();
                string value = element.Value;
                yield return new KeyValuePair<string, string>(key, value);
            }
        }

        public static KeyValueReader Create()
        {
            return new KeyValueReader();
        }
    }
}