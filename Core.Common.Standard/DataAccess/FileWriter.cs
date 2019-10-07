namespace KY.Core.DataAccess
{
    public interface IFileWriter<in T>
    {
        void Write(string path, T value);
    }

    public abstract class FileWriter<T> : IFileWriter<T>
    {
        protected IFileSystem FileSystem { get; }

        protected FileWriter(IFileSystem fileSystem)
        {
            this.FileSystem = fileSystem ?? new FileSystemInstance();
        }

        public void Write(string path, T value)
        {
            string fileContent = this.Serialize(value);
            this.FileSystem.WriteAllText(path, fileContent);
        }

        protected abstract string Serialize(T value);
    }
}