using System.Transactions;

namespace WalletManagement.Api.Models
{
    public class Wallet
    {
        public int Id {  get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
