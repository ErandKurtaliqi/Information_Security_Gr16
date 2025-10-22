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

            byte[] keyBytes = DeriveKey(password);
            byte[] iv = GenerateRandomBytes(BlockSize);

            // BF = Blowfish, CBC = Cipher Block Chaining, PKCS7 = padding
            IBufferedCipher cipher = CipherUtilities.GetCipher("BF/CBC/PKCS7");
            cipher.Init(true, new ParametersWithIV(new KeyParameter(keyBytes), iv));

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = cipher.DoFinal(inputBytes);

            // Ruajmë IV në fillim të stream-it (IV + ciphertext)
            byte[] result = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

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
