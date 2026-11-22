using Microsoft.EntityFrameworkCore;
using WalletManagement.Api.Data;
using WalletManagement.Api.Models;
using WalletManagement.Api.Queries;
using Xunit;

namespace WalletManagement.Tests
{
    public class WalletQueryServiceTests
    {
        private WalletDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new WalletDbContext(options);

            var wallet1 = new Wallet { Id = 1, OwnerName = "Alice", Balance = 1500m, CreatedAt = DateTime.UtcNow };
            var wallet2 = new Wallet { Id = 2, OwnerName = "Bob", Balance = 300m, CreatedAt = DateTime.UtcNow };

            context.Wallets.AddRange(wallet1, wallet2);

            context.Transactions.AddRange(
                new Transaction { Id = 1, WalletId = 1, Type = "Deposit", Amount = 500m, CreatedAt = DateTime.UtcNow },
                new Transaction { Id = 2, WalletId = 1, Type = "Withdrawal", Amount = 100m, CreatedAt = DateTime.UtcNow },
                new Transaction { Id = 3, WalletId = 2, Type = "Deposit", Amount = 300m, CreatedAt = DateTime.UtcNow }
            );

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetHighValueWalletsAsync_ReturnsWalletsAboveThreshold()
        {
            var context = GetInMemoryDbContext();
            var queryService = new WalletQueryService(context);

            var result = await queryService.GetHighValueWalletsAsync(1000m);

            var walletList = result.ToList();
            Assert.Single(walletList);
            Assert.Equal("Alice", walletList[0].OwnerName);
        }

        [Fact]
        public async Task GetTotalSystemLiquidityAsync_ReturnsCorrectSum()
        {
            var context = GetInMemoryDbContext();
            var queryService = new WalletQueryService(context);

            var totalLiquidity = await queryService.GetTotalSystemLiquidityAsync();

            Assert.Equal(1800m, totalLiquidity);
        }
    }
}