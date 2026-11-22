
using Microsoft.EntityFrameworkCore;
using WalletManagement.Api.Data;
using WalletManagement.Api.DTOs;
using WalletManagement.Api.Models;

namespace WalletManagement.Api.Services;

public class WalletService(WalletDbContext context) : IWalletService
{
    public async Task<IEnumerable<WalletResponseDto>> GetAllWalletsAsync()
    {
        return await context.Wallets
            .Include(w => w.Transactions)
            .Select(w => new WalletResponseDto
            {
                Id = w.Id,
                OwnerName = w.OwnerName,
                Balance = w.Balance,
                CreatedAt = w.CreatedAt,
                Transactions = w.Transactions.Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    WalletId = t.WalletId,
                    Type = t.Type,
                    Amount = t.Amount,
                    CreatedAt = t.CreatedAt
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<WalletResponseDto?> GetWalletByIdAsync(int id)
    {
        var wallet = await context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (wallet == null) return null;

        return new WalletResponseDto
        {
            Id = wallet.Id,
            OwnerName = wallet.OwnerName,
            Balance = wallet.Balance,
            CreatedAt = wallet.CreatedAt,
            Transactions = wallet.Transactions.Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                WalletId = t.WalletId,
                Type = t.Type,
                Amount = t.Amount,
                CreatedAt = t.CreatedAt
            }).ToList()
        };
    }

    public async Task<WalletResponseDto> CreateWalletAsync(CreateWalletDto dto)
    {
        var now = DateTime.UtcNow;

        var wallet = new Wallet
        {
            OwnerName = dto.OwnerName,
            Balance = dto.InitialBalance,
            CreatedAt = now
        };

        if (dto.InitialBalance > 0)
        {
            wallet.Transactions.Add(new Transaction
            {
                Type = "Credit",
                Amount = dto.InitialBalance,
                CreatedAt = now
            });
        }

        context.Wallets.Add(wallet);
        await context.SaveChangesAsync();

        return new WalletResponseDto
        {
            Id = wallet.Id,
            OwnerName = wallet.OwnerName,
            Balance = wallet.Balance,
            CreatedAt = wallet.CreatedAt,
            Transactions = wallet.Transactions.Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                WalletId = t.WalletId,
                Type = t.Type,
                Amount = t.Amount,
                CreatedAt = t.CreatedAt
            }).ToList()
        };
    }

    public async Task<bool> DepositAsync(int walletId, decimal amount)
    {
        var wallet = await context.Wallets.FindAsync(walletId);
        if (wallet == null || amount <= 0) return false;

        wallet.Balance += amount;

        context.Transactions.Add(new Transaction
        {
            WalletId = walletId,
            Type = "Credit",
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> WithdrawAsync(int walletId, decimal amount)
    {
        var wallet = await context.Wallets.FindAsync(walletId);
        if (wallet == null || amount <= 0 || wallet.Balance < amount) return false;

        wallet.Balance -= amount;

        context.Transactions.Add(new Transaction
        {
            WalletId = walletId,
            Type = "Debit",
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
        return true;
    }
}