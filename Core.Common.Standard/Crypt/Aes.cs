using System;
using System.Security.Cryptography;
using System.Text;

namespace KY.Core.Crypt
{
    public class Aes
    {
        private const int Iterations = 1000;
        private readonly RijndaelManaged crypter = new RijndaelManaged();
        private readonly byte[] salt;

        public Aes(string password)
        {
            this.crypter.BlockSize = 128;
            this.crypter.KeySize = 128;

            this.crypter.Padding = PaddingMode.PKCS7;
            this.crypter.Mode = CipherMode.CBC;
            this.salt = Encoding.UTF8.GetBytes("4ofl49ds0o4903dfl49dfl3oaakf902i34");
            this.crypter.Key = this.GenerateKey(password);
            this.crypter.IV = this.crypter.Key;
        }

        public string Encrypt(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            ICryptoTransform transform = this.crypter.CreateEncryptor();
            byte[] encryptedBytes = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string value)
        {
            byte[] encryptedBytes = Convert.FromBase64String(value);
            ICryptoTransform transform = this.crypter.CreateDecryptor(this.crypter.Key, this.crypter.IV);
            byte[] bytes = transform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        private byte[] GenerateKey(string password)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), this.salt, Iterations);
            return rfc2898.GetBytes(128 / 8);
        }

        public static Aes Create(string password)
        {
            return new Aes(password);
        }
    }
}