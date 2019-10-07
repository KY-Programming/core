using System;
using System.Security.Cryptography;

namespace KY.Core.Crypt
{
    [Obsolete("Use Sha256 instead")]
    public class Sha1 : Hash
    {
        public Sha1(string hash)
            : base(hash)
        { }

        private Sha1(HashAlgorithm hash, string value)
            : base(hash, value)
        { }

        public static Sha1 Create(string value)
        {
            using (SHA1Managed hash = new SHA1Managed())
            {
                return new Sha1(hash, value);
            }
        }
    }
}