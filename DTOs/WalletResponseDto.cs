namespace WalletManagement.Api.DTOs
{
    public class WalletResponseDto
    {
        public int Id { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TransactionResponseDto> Transactions { get; set; } = [];
    }
}
