using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Text;

namespace ProjectI.Interfaces
{
    public interface IBlowfishService
    {
        public Task<string> DecryptFromBase64Async(string base64Cipher, byte[] key, byte[] iv8)
        {
            if (iv8.Length != throw new ArgumentException("IV duhet 8 bajte.", nameof(iv8));
            byte[] cipherBytes = Convert.FromBase64String(base64Cipher);

            IBlockCipher engine = new BlowfishEngine();
            IBlockCipher cbc = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(cbc, new Pkcs7Padding());

            var keyParam = new KeyParameter(key);
            var keyParamWithIV = new ParametersWithIV(keyParam, iv8);
            cipher.Init(false, keyParamWithIV);

            byte[] output = new byte[cipher.GetOutputSize(cipherBytes.Length)];
            int len = cipher.ProcessBytes(cipherBytes, 0, cipherBytes.Length, output, 0);
            len += cipher.DoFinal(output, len);

            var plain = new byte[len];
            Array.Copy(output, 0, plain, 0, len);
            return Task.FromResult(Encoding.UTF8.GetString(plain));
        }

        private static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 != 0) throw new ArgumentException("Hex me gjatësi çift pritet.");
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }
    }
}
