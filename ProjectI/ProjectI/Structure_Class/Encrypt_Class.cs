using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

namespace ProjectI.Structure_Class
{
    public class Encrypt_Class
    {
        public string Encrypt(string plainText, string password)
        {
            if (plainText == null)
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

           

            return Convert.ToBase64String(result);
        }
        private byte[] DeriveKey(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Mund ta shkurtosh ose zgjatësh sipas nevojës (maks 56 byte për Blowfish)
                byte[] key = new byte[32]; // 32 byte = 256 bit
                Buffer.BlockCopy(hash, 0, key, 0, key.Length);
                return key;


            }
        }



        private byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }
    }
}
