using System.IO;
using System.Text;
using System.Xml.Linq;

namespace KY.Core.DataAccess
{
    public interface IFileSystem
    {
        string Root { get; }
        string[] GetDirectories(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly);
        DirectoryInfo GetDirectoryInfo(params string[] pathChunks);
        DirectoryInfo[] GetDirectoryInfos(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly);
        bool DirectoryExists(params string[] pathChunks);
        void CreateDirectory(params string[] pathChunks);
        void MoveDirectory(string path, string oldDirectoryName, string newDirectoryName, bool overwrite = false);
        void MoveDirectory(string from, string to, bool overwrite = false);
        void DeleteDirectory(params string[] pathChunks);
        void CopyDirectory(string from, string to, bool overwrite = false);
        FileInfo GetFileInfo(string path);
        FileInfo[] GetFileInfos(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly);
        string[] GetFiles(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly);
        string ReadAllText(string path, Encoding encoding = null);
        string ReadAllText(string path, string fileName, Encoding encoding = null);
        void WriteAllText(string path, string content, Encoding encoding = null);
        void WriteAllText(string path, string fileName, string content, Encoding encoding = null);
        void AppendAllText(string path, string content, Encoding encoding = null);
        void WriteAllBytes(string path, byte[] content);
        bool FileExists(params string[] pathChunks);
        void DeleteFile(params string[] pathChunks);
        string[] ReadAllLines(string path, Encoding encoding = null);
        XElement ReadXml(params string[] pathChunks);
        Stream OpenRead(params string[] pathChunks);
        Stream OpenWrite(params string[] pathChunks);
        void CopyFile(string from, string to, bool overwrite = false);
        void MoveFile(string path, string oldFileName, string newFileName, bool overwrite = false);
        void MoveFile(string from, string to, bool overwrite = false);
        string FormatFileSize(long size);
        bool IsAbsolute(string path);
        string ToAbsolutePath(string path);
        string ToRelativePath(string path, bool useRelativeChar = true);
        string FormatPath(string path);
        string Combine(params string[] pathChunks);
        string Parent(string path);
        DirectoryInfo ParentInfo(string path);
        string ToDirectory(string directory);
        long ParseFileSize(string value);
        string GetFileName(string path);
        string GetDirectoryName(string path);
    }
}