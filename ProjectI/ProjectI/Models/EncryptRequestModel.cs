using System.ComponentModel.DataAnnotations;

namespace ProjectI.Models
{
    public sealed class EncryptRequestModel
    {
        [Required] public string Plaintext { get; set; } = "";
        public string? KeyBase64 { get; set; }
        public string? IVHex { get; set; }
    }
}
