using System.Security.Cryptography;

namespace KY.Core.Crypt
{
    public class Sha512 : Hash
    {
        public Sha512(string hash)
            : base(hash)
        { }

        private Sha512(HashAlgorithm hash, string value)
            : base(hash, value)
        { }

        public static Sha512 Create(string value)
        {
            using (SHA512Managed hash = new SHA512Managed())
            {
                return new Sha512(hash, value);
            }
        }
    }
}