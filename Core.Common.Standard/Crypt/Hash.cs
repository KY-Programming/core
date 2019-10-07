using System.Security.Cryptography;
using System.Text;

namespace KY.Core.Crypt
{
    public abstract class Hash
    {
        private readonly string value;

        protected Hash(string value)
        {
            this.value = value;
        }

        protected Hash(HashAlgorithm hash, string value)
        {
            byte[] buffer = hash.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in buffer)
            {
                builder.AppendFormat("{0:x2}", b);
            }
            this.value = builder.ToString();
        }

        public sealed override string ToString()
        {
            return this.value;
        }

        public sealed override bool Equals(object obj)
        {
            Hash other = obj as Hash;
            return (other != null) && string.Equals(this.value, other.value);
        }

        public sealed override int GetHashCode()
        {
            return this.value?.GetHashCode() ?? 0;
        }
    }
}