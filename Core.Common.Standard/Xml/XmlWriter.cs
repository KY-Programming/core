using System.Xml.Linq;
using KY.Core.DataAccess;

namespace KY.Core.Xml
{
    public abstract class XmlWriter<T> : FileWriter<T>
    {
        protected XmlWriter(IFileSystem fileSystem = null)
            : base(fileSystem)
        { }

        protected sealed override string Serialize(T value)
        {
            return this.SerializeToXml(value)?.ToString();
        }

        protected abstract XElement SerializeToXml(T value);
    }
}