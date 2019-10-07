using System;

namespace KY.Core.DataAccess
{
    public interface IFileReader<out T>
    {
        T Read(string path);
        T TryRead(string path);
        T Parse(string fileContent);
    }

    public abstract class FileReader<T> : IFileReader<T>
    {   
        protected IFileSystem FileSystem { get; }

        protected FileReader(IFileSystem fileSystem = null)
        {
            this.FileSystem = fileSystem ?? new FileSystemInstance();
        }

        public T Read(string path)
        {
            string fileContent = this.FileSystem.ReadAllText(path);
            return this.Parse(path, fileContent);
        }

        public T TryRead(string path)
        {
            if (this.FileSystem.FileExists(path)) 
                return this.Read(path);
            return default(T);
        }

        public T Parse(string fileContent)
        {
            return this.Parse(null, fileContent);
        }

        public abstract T Parse(string path, string fileContent);
    }
}