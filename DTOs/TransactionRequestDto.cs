using System.ComponentModel.DataAnnotations;

namespace WalletManagement.Api.DTOs
{
    public class TransactionRequestDto
    {
        [Range(0.01, 1000000000, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}
