using Microsoft.EntityFrameworkCore;
using WalletManagement.Api.Models;

namespace WalletManagement.Api.Data;

public class WalletDbContext(DbContextOptions<WalletDbContext> options)
    : DbContext(options)
{
    public DbSet<Wallet> Wallets => Set<Wallet>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Precision configurations
        modelBuilder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        // Relationships
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed sample Wallets (using lowercase 'id' and 'OwnerName')
        modelBuilder.Entity<Wallet>().HasData(
            new Wallet
            {
                id = 1,
                OwnerName = "Kelvin Jaston",
                Balance = 50000.00m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Wallet
            {
                id = 2,
                OwnerName = "John Doe",
                Balance = 150000.00m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed sample Transactions (matched strictly to your Transaction model properties)
        modelBuilder.Entity<Transaction>().HasData(
            new Transaction
            {
                Id = 1,
                WalletId = 1,
                Type = "Credit",
                Amount = 10000.00m,
                CreatedAt = new DateTime(2026, 1, 2, 10, 30, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 2,
                WalletId = 1,
                Type = "Debit",
                Amount = 5000.00m,
                CreatedAt = new DateTime(2026, 1, 3, 14, 15, 0, DateTimeKind.Utc)
            }
        );
    }
}