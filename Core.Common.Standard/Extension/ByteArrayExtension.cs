using System.Text;

namespace KY.Core.Extension
{
    public static class ByteArrayExtension
    {
        public static string Convert(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }
    }
}