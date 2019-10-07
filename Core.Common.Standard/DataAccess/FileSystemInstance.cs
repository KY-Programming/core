using System.IO;
using System.Text;
using System.Xml.Linq;

namespace KY.Core.DataAccess
{
    public class FileSystemInstance : IFileSystem
    {
        private readonly FileHelper fileHelper;
        private readonly DirectoryHelper directoryHelper;
        private readonly PathHelper pathHelper;

        public string Root => this.pathHelper.Root;

        // Wird von Ninject benötigt
        public FileSystemInstance()
            : this(null)
        { }

        public FileSystemInstance(string root)
        {
            this.pathHelper = new PathHelper(root);
            this.fileHelper = new FileHelper(this.pathHelper);
            this.directoryHelper = new DirectoryHelper(this.pathHelper);
        }

        public string[] GetDirectories(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            return this.directoryHelper.GetDirectories(path, searchPattern, option);
        }

        public DirectoryInfo GetDirectoryInfo(params string[] pathChunks)
        {
            return this.directoryHelper.GetDirectoryInfo(pathChunks);
        }

        public DirectoryInfo[] GetDirectoryInfos(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            return this.directoryHelper.GetDirectoryInfos(path, searchPattern, option);
        }

        public bool DirectoryExists(params string[] pathChunks)
        {
            return this.directoryHelper.Exists(pathChunks);
        }

        public void CreateDirectory(params string[] pathChunks)
        {
            this.directoryHelper.Create(pathChunks);
        }

        public void MoveDirectory(string path, string oldDirectoryName, string newDirectoryName, bool overwrite = false)
        {
            this.MoveDirectory(this.pathHelper.Combine(path, oldDirectoryName), this.pathHelper.Combine(path, newDirectoryName), overwrite);
        }

        public void MoveDirectory(string from, string to, bool overwrite = false)
        {
            this.directoryHelper.Move(from, to, overwrite);
        }

        public void DeleteDirectory(params string[] pathChunks)
        {
            this.directoryHelper.Delete(pathChunks);
        }

        public void CopyDirectory(string from, string to, bool overwrite = false)
        {
            this.directoryHelper.Copy(from, to, overwrite);
        }

        public FileInfo GetFileInfo(string path)
        {
            return this.fileHelper.GetInfo(path);
        }

        public FileInfo[] GetFileInfos(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            return this.fileHelper.GetInfos(path, searchPattern, option);
        }

        public string[] GetFiles(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            return this.fileHelper.GetFiles(path, searchPattern, option);
        }

        public string ReadAllText(string path, Encoding encoding = null)
        {
            return this.fileHelper.ReadAllText(path, encoding);
        }

        public string ReadAllText(string path, string fileName, Encoding encoding = null)
        {
            return this.ReadAllText(this.pathHelper.Combine(path, fileName), encoding);
        }

        public void WriteAllText(string path, string content, Encoding encoding = null)
        {
            this.fileHelper.WriteAllText(path, content, encoding);
        }

        public void WriteAllText(string path, string fileName, string content, Encoding encoding = null)
        {
            this.WriteAllText(this.pathHelper.Combine(path, fileName), content, encoding);
        }

        public void AppendAllText(string path, string content, Encoding encoding = null)
        {
            this.fileHelper.AppendAllText(path, content, encoding);
        }

        public void WriteAllBytes(string path, byte[] content)
        {
            this.fileHelper.WriteAllBytes(path, content);
        }

        public bool FileExists(params string[] pathChunks)
        {
            return this.fileHelper.Exists(pathChunks);
        }

        public void DeleteFile(params string[] pathChunks)
        {
            this.fileHelper.Delete(pathChunks);
        }

        public string[] ReadAllLines(string path, Encoding encoding = null)
        {
            return this.fileHelper.ReadAllLines(path, encoding);
        }

        public XElement ReadXml(params string[] pathChunks)
        {
            return this.fileHelper.ReadXml(pathChunks);
        }

        public Stream OpenRead(params string[] pathChunks)
        {
            return this.fileHelper.OpenRead(pathChunks);
        }

        public Stream OpenWrite(params string[] pathChunks)
        {
            return this.fileHelper.OpenWrite(pathChunks);
        }

        public void CopyFile(string from, string to, bool overwrite = false)
        {
            this.fileHelper.Copy(from, to, overwrite);
        }

        public void MoveFile(string path, string oldFileName, string newFileName, bool overwrite = false)
        {
            this.fileHelper.Move(path, oldFileName, newFileName, overwrite);
        }

        public void MoveFile(string from, string to, bool overwrite = false)
        {
            this.fileHelper.Move(from, to, overwrite);
        }

        public string FormatFileSize(long size)
        {
            return this.fileHelper.FormatSize(size);
        }

        public bool IsAbsolute(string path)
        {
            return this.pathHelper.IsAbsolute(path);
        }

        public string ToAbsolutePath(string path)
        {
            return this.pathHelper.ToAbsolute(path);
        }

        public string ToRelativePath(string path, bool useRelativeChar = true)
        {
            return this.pathHelper.ToRelative(path, useRelativeChar);
        }

        public string FormatPath(string path)
        {
            return this.pathHelper.Format(path);
        }

        public string Combine(params string[] pathChunks)
        {
            return this.pathHelper.Combine(pathChunks);
        }

        public string Parent(string path)
        {
            return this.pathHelper.Parent(path);
        }

        public DirectoryInfo ParentInfo(string path)
        {
            return new DirectoryInfo(this.pathHelper.Parent(path));
        }

        public string ToDirectory(string directory)
        {
            return this.directoryHelper.ToDirectory(directory);
        }

        public long ParseFileSize(string value)
        {
            return this.fileHelper.ParseSize(value);
        }

        public string GetFileName(string path)
        {
            return this.fileHelper.GetName(path);
        }

        public string GetDirectoryName(string path)
        {
            return this.directoryHelper.GetName(path);
        }

        protected string RemoveRelativeChar(string path)
        {
            return this.pathHelper.RemoveRelativeChar(path);
        }
    }
}