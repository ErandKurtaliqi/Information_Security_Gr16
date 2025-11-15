using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectI.Interfaces;
using ProjectI.Models;

namespace ProjectI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlowfishController : ControllerBase
    {
        [HttpPost("encrypt")]
        public async Task<ActionResult<CryptoResponseModel>> Encrypt([FromBody] EncryptRequestModel req)
        {
            try
            {
                string result;

                var keyBase64 = string.IsNullOrWhiteSpace(req.KeyBase64)
                    ? _config["Blowfish:KeyBase64"]
                    : req.KeyBase64;

                var ivHex = string.IsNullOrWhiteSpace(req.IVHex)
                    ? _config["Blowfish:IVHex"]
                    : req.IVHex;

                var key = Convert.FromBase64String(keyBase64!);
                var iv = HexToBytes(ivHex!);

                if (iv.Length != 8)
                    return BadRequest("IV duhet 8 bajte (16 hexdigjit).");

                result = await _svc.EncryptToBase64Async(req.Plaintext, key, iv);
                return Ok(new CryptoResponseModel { Result = result });
            }
            catch (Exception ex)
            {
                return Problem($"Encrypt error: {ex.Message}");
            }
        }


        [HttpPost("decrypt")]
        public async Task<ActionResult<CryptoResponseModel>> Decrypt([FromBody] DecryptRequestModel req)
        {
            try
            {
                string result;
                if (!string.IsNullOrWhiteSpace(req.KeyBase64) && !string.IsNullOrWhiteSpace(req.IVHex))
                {
                    var key = Convert.FromBase64String(req.KeyBase64);
                    var iv = HexToBytes(req.IVHex);
                    if (iv.Length != 8) return BadRequest("IV duhet 8 bajte (16 hexdigjit).");
                    result = await _svc.DecryptFromBase64Async(req.CipherBase64, key, iv);
                }
                else
                {
                    result = await _svc.DecryptFromBase64Async(req.CipherBase64);
                }
                return Ok(new CryptoResponseModel { Result = result });
            }
            catch (Exception ex)
            {
                return Problem($"Decrypt error: {ex.Message}");
            }
        }

    }
}
