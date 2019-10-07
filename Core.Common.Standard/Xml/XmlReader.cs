using System.Xml.Linq;
using KY.Core.DataAccess;

namespace KY.Core.Xml
{
    public abstract class XmlReader<T> : FileReader<T>
    {
        protected XmlReader(IFileSystem fileSystem = null)
            : base(fileSystem)
        { }

        public sealed override T Parse(string path, string fileContent)
        {
            XElement rootElement = XElement.Parse(fileContent);
            return this.Parse(path, rootElement);
        }

        public T Parse(XElement rootElement)
        {
            return this.Parse(null, rootElement);
        }

        public abstract T Parse(string path, XElement rootElement);
    }
}