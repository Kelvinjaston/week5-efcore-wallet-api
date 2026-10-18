using System.ComponentModel.DataAnnotations;

namespace WalletManagement.Api.DTOs
{
    public class CreateWalletDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string OwnerName { get; set; } = string.Empty;

        [Range(0, 1000000000, ErrorMessage = "Initial balance must be zero or positive.")]
        public decimal InitialBalance { get; set; } = 0m;
    }
}
