using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KY.Core
{
    public static class StreamExtension
    {
        public static void Write(this Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }

        public static async Task WriteAsync(this Stream stream, string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            await stream.WriteAsync(buffer, 0, buffer.Length);
            await stream.FlushAsync();
        }

        public static async Task<string> ReadStringAsync(this Stream stream)
        {
            StringBuilder builder = new StringBuilder();
            byte[] buffer = new byte[1024];
            int bytesRead = buffer.Length;
            while (bytesRead == buffer.Length)
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                builder.Append(Encoding.UTF8.GetString(buffer).TrimEnd('\0'));
            }
            return builder.ToString();
        }
    }
}