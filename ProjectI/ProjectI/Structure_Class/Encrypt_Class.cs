using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using ProjectI.Config;
using ProjectI.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ProjectI.Structure_Class
{
    public class BlowfishService : IBlowfishService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv; 

        public BlowfishService(IOptions<BlowfishOptions> options)
        {
            var opt = options.Value ?? throw new ArgumentNullException(nameof(options));
            _key = Convert.FromBase64String(opt.KeyBase64 ?? throw new InvalidOperationException("KeyBase64 mungon"));
            _iv = HexToBytes(opt.IVHex ?? throw new InvalidOperationException("IVHex mungon"));
            if (_iv.Length != 8) throw new InvalidOperationException("IV duhet të jetë fiks 8 bajte.");
            if (_key.Length < 4 || _key.Length > 56) throw new InvalidOperationException("Blowfish key duhet 4–56 bajte.");
        }

        public Task<string> EncryptToBase64Async(string plaintext)
            => EncryptToBase64Async(plaintext, _key, _iv);

        public Task<string> DecryptFromBase64Async(string base64Cipher)
            => DecryptFromBase64Async(base64Cipher, _key, _iv);

        public Task<string> EncryptToBase64Async(string plaintext, byte[] key, byte[] iv8)
        {
            if (iv8.Length != 8) throw new ArgumentException("IV duhet 8 bajte.", nameof(iv8));
            byte[] input = Encoding.UTF8.GetBytes(plaintext);

            IBlockCipher engine = new BlowfishEngine();
            IBlockCipher cbc = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(cbc, new Pkcs7Padding());

            var keyParam = new KeyParameter(key);
            var keyParamWithIV = new ParametersWithIV(keyParam, iv8);
            cipher.Init(true, keyParamWithIV);

            byte[] output = new byte[cipher.GetOutputSize(input.Length)];
            int len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
            len += cipher.DoFinal(output, len);

            var finalBytes = new byte[len];
            Array.Copy(output, 0, finalBytes, 0, len);
            return Task.FromResult(Convert.ToBase64String(finalBytes));
        }

        public Task<string> DecryptFromBase64Async(string base64Cipher, byte[] key, byte[] iv8)
        {
            if (iv8.Length != 8) throw new ArgumentException("IV duhet 8 bajte.", nameof(iv8));
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
