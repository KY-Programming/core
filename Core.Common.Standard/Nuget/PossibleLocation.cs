namespace KY.Core
{
    internal class PossibleLocation
    {
        public string Path { get; }
        public SemanticVersion Version { get; }

        public PossibleLocation(string path, SemanticVersion version)
        {
            this.Path = path;
            this.Version = version;
        }
    }
}
