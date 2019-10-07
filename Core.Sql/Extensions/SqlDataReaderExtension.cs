using System.Data.SqlClient;

namespace KY.Core.Sql.Extensions
{
    public static class SqlDataReaderExtension
    {
        public static int GetInt32(this SqlDataReader reader, int ordinal, int nullValue)
        {
            return reader.IsDBNull(ordinal) ? nullValue : reader.GetInt32(ordinal);
        }

        public static string GetString(this SqlDataReader reader, int ordinal, string nullValue)
        {
            return reader.IsDBNull(ordinal) ? nullValue : reader.GetString(ordinal);
        }

        public static byte GetByte(this SqlDataReader reader, int ordinal, byte nullValue)
        {
            return reader.IsDBNull(ordinal) ? nullValue : reader.GetByte(ordinal);
        }
    }
}
