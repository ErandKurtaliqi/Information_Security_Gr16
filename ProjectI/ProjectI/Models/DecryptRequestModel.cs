using System.ComponentModel.DataAnnotations;

namespace ProjectI.Models
{
    public sealed class DecryptRequestModel
    {
        [Required] public string CipherBase64 { get; set; } = "";
        public string? KeyBase64 { get; set; }
        public string? IVHex { get; set; }
    }
}