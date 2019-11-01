using System;

namespace KY.Core
{
    internal class AssemblyInfo
    {
        public string Name { get; }
        public Version Version { get; }

        public AssemblyInfo(string name, Version version = null)
        {
            this.Name = name;
            this.Version = version;
        }
    }
}