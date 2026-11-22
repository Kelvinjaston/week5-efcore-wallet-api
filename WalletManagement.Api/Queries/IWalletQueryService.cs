using WalletManagement.Api.DTOs;
using WalletManagement.Api.Models;

namespace WalletManagement.Api.Queries
{
    public interface IWalletQueryService
    {
        Task<IEnumerable<WalletResponseDto>> GetHighValueWalletsAsync(decimal minBalance);

        Task<decimal> GetTotalSystemLiquidityAsync();

        Task<IEnumerable<object>> GetMostActiveWalletsAsync(int topCount);

        Task<IEnumerable<object>> GetTopLargeTransactionsAsync(int topCount);

        Task<IEnumerable<WalletResponseDto>> GetDormantWalletsAsync();

        Task<IEnumerable<object>> GetWalletCashFlowAsync();

        Task<IEnumerable<TransactionResponseDto>> GetTransactionsByDateRangeAsync(int walletId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<WalletResponseDto>> GetRecentHighBalanceWalletsAsync(decimal minBalance, int days);
    }
}