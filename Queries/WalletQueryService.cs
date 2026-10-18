using Microsoft.EntityFrameworkCore;
using WalletManagement.Api.Data;
using WalletManagement.Api.DTOs;
using WalletManagement.Api.Models;

namespace WalletManagement.Api.Queries
{
    public class WalletQueryService : IWalletQueryService
    {
        private readonly WalletDbContext _context;

        public WalletQueryService(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WalletResponseDto>> GetHighValueWalletsAsync(decimal minBalance)
        {
            return await _context.Wallets
                .Where(w => w.Balance >= minBalance)
                .OrderByDescending(w => w.Balance)
                .Select(w => new WalletResponseDto
                {
                    Id = w.Id,
                    OwnerName = w.OwnerName,
                    Balance = w.Balance,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSystemLiquidityAsync()
        {
            return await _context.Wallets.SumAsync(w => w.Balance);
        }

        public async Task<IEnumerable<object>> GetMostActiveWalletsAsync(int topCount)
        {
            return await _context.Transactions
                .GroupBy(t => t.WalletId)
                .Select(g => new
                {
                    WalletId = g.Key,
                    TransactionCount = g.Count()
                })
                .OrderByDescending(x => x.TransactionCount)
                .Take(topCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetTopLargeTransactionsAsync(int topCount)
        {
            return await _context.Transactions
                .Join(_context.Wallets,
                    t => t.WalletId,
                    w => w.Id,
                    (t, w) => new
                    {
                        TransactionId = t.Id,
                        OwnerName = w.OwnerName,
                        t.Type,
                        t.Amount,
                        t.CreatedAt
                    })
                .OrderByDescending(x => x.Amount)
                .Take(topCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<WalletResponseDto>> GetDormantWalletsAsync()
        {
            return await _context.Wallets
                .Where(w => !w.Transactions.Any())
                .Select(w => new WalletResponseDto
                {
                    Id = w.Id,
                    OwnerName = w.OwnerName,
                    Balance = w.Balance,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetWalletCashFlowAsync()
        {
            return await _context.Transactions
                .GroupBy(t => t.WalletId)
                .Select(g => new
                {
                    WalletId = g.Key,
                    TotalDeposits = g.Where(t => t.Type == "Deposit").Sum(t => (decimal?)t.Amount) ?? 0,
                    TotalWithdrawals = g.Where(t => t.Type == "Withdrawal").Sum(t => (decimal?)t.Amount) ?? 0
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetTransactionsByDateRangeAsync(int walletId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .OrderBy(t => t.CreatedAt)
                .Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    WalletId = t.WalletId,
                    Type = t.Type,
                    Amount = t.Amount,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<WalletResponseDto>> GetRecentHighBalanceWalletsAsync(decimal minBalance, int days)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            return await _context.Wallets
                .Where(w => w.CreatedAt >= cutoffDate && w.Balance >= minBalance)
                .Select(w => new WalletResponseDto
                {
                    Id = w.Id,
                    OwnerName = w.OwnerName,
                    Balance = w.Balance,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();
        }
    }
}