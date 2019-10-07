using System.Security.Cryptography;

namespace KY.Core.Crypt
{
    public class Md5 : Hash
    {
        public Md5(string hash)
            : base(hash)
        { }

        private Md5(HashAlgorithm hash, string value)
            : base(hash, value)
        { }

        public static Md5 Create(string value)
        {
            using (MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider())
            {
                return new Md5(hash, value);
            }
        }
    }
}