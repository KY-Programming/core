namespace KY.Core.Nuget
{
    public class SearchLocation
    {
        public string Path { get; set; }
        public bool SearchLocal { get; set; } = true;
        public bool SearchBin { get; set; } = true;
        public bool SearchBinDebug { get; set; } = true;
        public bool SearchBinRelease { get; set; } = true;
        public bool SearchByVersion { get; set; } = true;

        public SearchLocation(string path)
        {
            this.Path = path;
        }

        public SearchLocation SetPath(string path)
        {
            this.Path = path;
            return this;
        }

        public SearchLocation Reset()
        {
            this.SearchLocal = false;
            this.SearchBin = false;
            this.SearchBinDebug = false;
            this.SearchBinRelease = false;
            this.SearchByVersion = false;
            return this;
        }

        public SearchLocation SearchOnlyByVersion()
        {
            this.Reset();
            this.SearchByVersion = true;
            return this;
        }

        public SearchLocation SearchOnlyLocal()
        {
            this.Reset();
            this.SearchLocal = true;
            return this;
        }
    }
}