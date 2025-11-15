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
        Task<string> EncryptToBase64Async(string plaintext);
        Task<string> DecryptFromBase64Async(string base64Cipher);
        Task<string> EncryptToBase64Async(string plaintext, byte[] key, byte[] iv8);
        Task<string> DecryptFromBase64Async(string base64Cipher, byte[] key, byte[] iv8);
    }
}
