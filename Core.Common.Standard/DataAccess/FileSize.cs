using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace KY.Core.DataAccess
{
    public class FileSize
    {
        private readonly long size;

        private static readonly Regex parseRegex = new Regex(@"^(?<size>\d+([.,]\d+)?)(?<unit>(b|kb|k|mb|m|gb|g|t|tb))$", RegexOptions.IgnoreCase);

        public FileSize(long? size = null)
        {
            this.size = size ?? 0;
        }

        public static bool operator >=(long left, FileSize right)
        {
            return right != null && left >= right.size;
        }

        public static bool operator <=(long left, FileSize right)
        {
            return right != null && left <= right.size;
        }

        public static bool operator >=(FileSize left, long right)
        {
            return left != null && left.size >= right;
        }

        public static bool operator <=(FileSize left, long right)
        {
            return left != null && left.size <= right;
        }

        public static bool operator >=(FileSize left, FileSize right)
        {
            return left == null && right == null || left != null && right != null && left.size >= right.size;
        }

        public static bool operator <=(FileSize left, FileSize right)
        {
            return left == null && right == null || left != null && right != null && left.size <= right.size;
        }

        public static bool operator >(long left, FileSize right)
        {
            return right != null && left > right.size;
        }

        public static bool operator <(long left, FileSize right)
        {
            return right != null && left < right.size;
        }

        public static bool operator >(FileSize left, long right)
        {
            return left != null && left.size > right;
        }

        public static bool operator <(FileSize left, long right)
        {
            return left != null && left.size < right;
        }

        public static bool operator >(FileSize left, FileSize right)
        {
            return left != null && right != null && left.size > right.size;
        }

        public static bool operator <(FileSize left, FileSize right)
        {
            return left != null && right != null && left.size < right.size;
        }

        public static bool operator ==(long left, FileSize right)
        {
            return right != null && right.size == left;
        }

        public static bool operator !=(long left, FileSize right)
        {
            return !(left == right);
        }

        public static bool operator ==(FileSize left, long right)
        {
            return left != null && left.size == right;
        }

        public static bool operator !=(FileSize left, long right)
        {
            return !(left == right);
        }

        public static bool operator ==(FileSize left, FileSize right)
        {
            return Equals(left, null) && Equals(right, null) || !Equals(left, null) && left.Equals(right);
        }

        public static bool operator !=(FileSize left, FileSize right)
        {
            return !(left == right);
        }

        public static FileSize operator +(FileSize left, FileSize right)
        {
            return new FileSize(left?.size + right?.size);
        }

        public static FileSize operator +(FileSize left, long right)
        {
            return new FileSize(left?.size + right);
        }

        public static FileSize operator +(long left, FileSize right)
        {
            return new FileSize(left + right?.size);
        }

        public static FileSize operator -(FileSize left, FileSize right)
        {
            return new FileSize(left?.size - right?.size);
        }

        public static FileSize operator -(FileSize left, long right)
        {
            return new FileSize(left?.size - right);
        }

        public static FileSize operator -(long left, FileSize right)
        {
            return new FileSize(left - right?.size);
        }

        public static FileSize operator *(FileSize left, FileSize right)
        {
            return new FileSize(left?.size * right?.size);
        }

        public static FileSize operator *(FileSize left, long right)
        {
            return new FileSize(left?.size * right);
        }

        public static FileSize operator *(long left, FileSize right)
        {
            return new FileSize(left * right?.size);
        }

        public static FileSize operator /(FileSize left, FileSize right)
        {
            return new FileSize(left?.size / right?.size);
        }

        public static FileSize operator /(FileSize left, long right)
        {
            return new FileSize(left?.size / right);
        }

        public static FileSize operator /(long left, FileSize right)
        {
            return new FileSize(left / right?.size);
        }

        public override bool Equals(object obj)
        {
            FileSize fileSize = obj as FileSize;
            return fileSize != null && fileSize.size == this.size;
        }

        public override int GetHashCode()
        {
            return this.size.GetHashCode();
        }

        public static implicit operator FileSize(long size)
        {
            return new FileSize(size);
        }

        public long ToByte()
        {
            return this.size;
        }

        public double ToKiloByte()
        {
            return this.size / 1024.0;
        }

        public double ToMegaByte()
        {
            return this.size / 1024.0 / 1024;
        }

        public double ToGigaByte()
        {
            return this.size / 1024.0 / 1024 / 1024;
        }

        public double ToTerraByte()
        {
            return this.size / 1024.0 / 1024 / 1024 / 1024;
        }

        public static FileSize Byte(long size)
        {
            return new FileSize(size);
        }

        public static FileSize KiloByte(double size)
        {
            return new FileSize((long)(size * 1024));
        }

        public static FileSize MegaByte(double size)
        {
            return new FileSize((long)(size * 1024 * 1024));
        }

        public static FileSize GigaByte(double size)
        {
            return new FileSize((long)(size * 1024 * 1024 * 1024));
        }

        public static FileSize TerraByte(double size)
        {
            return new FileSize((long)(size * 1024 * 1024 * 1024 * 1024));
        }

        public static FileSize Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new FileSize();

            Match match = parseRegex.Match(value);
            if (!match.Success)
            {
                throw new InvalidOperationException($"Can not parse value {value}");
            }
            long factor;
            switch (match.Groups["unit"].Value.ToLowerInvariant())
            {
                case "kb":
                case "k":
                    factor = 1024;
                    break;
                case "mb":
                case "m":
                    factor = 1024 * 1024;
                    break;
                case "gb":
                case "g":
                    factor = 1024 * 1024 * 1024;
                    break;
                case "tb":
                case "t":
                    factor = 1024L * 1024 * 1024 * 1024;
                    break;
                default:
                    factor = 1;
                    break;
            }
            double size = double.Parse(match.Groups["size"].Value.Replace(",", "."), CultureInfo.InvariantCulture);
            return new FileSize((long)(size * factor));
        }
    }
}