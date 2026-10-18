using Microsoft.AspNetCore.Mvc;
using WalletManagement.Api.Queries;

namespace WalletManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletQueriesController : ControllerBase
    {
        private readonly IWalletQueryService _queryService;

        public WalletQueriesController(IWalletQueryService queryService)
        {
            _queryService = queryService;
        }

        [HttpGet("high-value")]
        public async Task<IActionResult> GetHighValueWallets([FromQuery] decimal minBalance = 1000)
        {
            var result = await _queryService.GetHighValueWalletsAsync(minBalance);
            return Ok(result);
        }

        [HttpGet("liquidity")]
        public async Task<IActionResult> GetTotalSystemLiquidity()
        {
            var result = await _queryService.GetTotalSystemLiquidityAsync();
            return Ok(new { TotalLiquidity = result });
        }

        [HttpGet("most-active")]
        public async Task<IActionResult> GetMostActiveWallets([FromQuery] int topCount = 5)
        {
            var result = await _queryService.GetMostActiveWalletsAsync(topCount);
            return Ok(result);
        }

        [HttpGet("top-transactions")]
        public async Task<IActionResult> GetTopLargeTransactions([FromQuery] int topCount = 5)
        {
            var result = await _queryService.GetTopLargeTransactionsAsync(topCount);
            return Ok(result);
        }

        [HttpGet("dormant")]
        public async Task<IActionResult> GetDormantWallets()
        {
            var result = await _queryService.GetDormantWalletsAsync();
            return Ok(result);
        }

        [HttpGet("cash-flow")]
        public async Task<IActionResult> GetWalletCashFlow()
        {
            var result = await _queryService.GetWalletCashFlowAsync();
            return Ok(result);
        }

        [HttpGet("{walletId}/transactions-range")]
        public async Task<IActionResult> GetTransactionsByDateRange(int walletId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _queryService.GetTransactionsByDateRangeAsync(walletId, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("recent-high-balance")]
        public async Task<IActionResult> GetRecentHighBalanceWallets([FromQuery] decimal minBalance = 500, [FromQuery] int days = 30)
        {
            var result = await _queryService.GetRecentHighBalanceWalletsAsync(minBalance, days);
            return Ok(result);
        }
    }
}