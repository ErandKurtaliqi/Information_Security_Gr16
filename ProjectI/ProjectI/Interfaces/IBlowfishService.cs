namespace ProjectI.Interfaces
{
    public interface IBlowfishService
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
    }
}
