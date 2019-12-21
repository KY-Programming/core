using System;
using System.Globalization;
using System.Reflection;
using KY.Core.DataAccess;
using KY.Core.Properties;

namespace KY.Core
{
    public class FileTarget : LogTarget
    {
        private readonly object fileMutex = new object();
        public string Path { get; set; }

        public FileTarget()
        {
            this.Path = FileSystem.ToAbsolutePath("Logs");
        }

        public override void Write(LogEntry entry)
        {
            if (entry.Type == LogType.Error)
            {
                string formattedError = string.Format(CultureInfo.InvariantCulture, Resources.FileErrorFormat, entry.Timestamp, entry.CustomType, entry.Message);
                string errorFileName = FileSystem.Combine(this.Path, string.Format(CultureInfo.InvariantCulture, Resources.FileErrorFileName, entry.Timestamp));
                this.WriteFile(errorFileName, formattedError);
            }

            string formattedMessage = string.Format(CultureInfo.InvariantCulture, Resources.FileTraceFormat, entry.Timestamp, entry.CustomType, entry.Message);
            string fileName = FileSystem.Combine(this.Path, string.Format(CultureInfo.InvariantCulture, Resources.FileTraceFileName, entry.Timestamp));
            this.WriteFile(fileName, formattedMessage);
        }

        private void WriteFile(string fileName, string text)
        {
            try
            {
                FileSystem.CreateDirectory(this.Path);
                lock (this.fileMutex)
                {
                    FileSystem.AppendAllText(fileName, text);
                }
            }
            catch (Exception exception)
            {
                Logger.EventLog?.Write(LogEntry.Error(exception));
            }
        }

        public void SetPath(Assembly assembly)
        {
            this.Path = FileSystem.Combine(FileSystem.Parent(assembly.Location), "Logs");
        }
    }
}