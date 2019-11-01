using System;

namespace KY.Core
{
    internal class PossibleLocation
    {
        public string Path { get; }
        public Version Version { get; }

        public PossibleLocation(string path, Version version)
        {
            this.Path = path;
            this.Version = version;
        }
    }
}