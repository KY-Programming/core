using System.Xml.Linq;
using KY.Core.DataAccess;

namespace KY.Core
{
    public interface ISettingsService
    {
        XElement GetSection(string sectionName);
    }

    public class SettingsService : ISettingsService
    {
        private XElement root;

        public XElement GetSection(string sectionName)
        {
            if (this.root == null)
            {
                this.root = FileSystem.ReadXml("Settings.xml");
            }
            return this.root.Element(sectionName);
        }
    }
}