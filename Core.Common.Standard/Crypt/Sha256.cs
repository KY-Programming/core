using System.Security.Cryptography;

namespace KY.Core.Crypt
{
    public class Sha256 : Hash
    {
        public Sha256(string hash)
            : base(hash)
        { }

        private Sha256(HashAlgorithm hash, string value)
            : base(hash, value)
        { }

        public static Sha256 Create(string value)
        {
            using (SHA256Managed hash = new SHA256Managed())
            {
                return new Sha256(hash, value);
            }
        }
    }
}