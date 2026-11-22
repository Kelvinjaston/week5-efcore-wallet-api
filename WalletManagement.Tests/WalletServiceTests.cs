using Microsoft.EntityFrameworkCore;
using WalletManagement.Api.Data;
using WalletManagement.Api.DTOs;
using WalletManagement.Api.Models;
using WalletManagement.Api.Services;
using Xunit;

namespace WalletManagement.Tests
{
    public class WalletServiceTests
    {
        private static WalletDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new WalletDbContext(options);
        }

        [Fact]
        public async Task CreateWallet_WithInitialBalance_RecordsCreditTransaction()
        {
            var context = CreateContext();
            var service = new WalletService(context);

            var result = await service.CreateWalletAsync(
                new CreateWalletDto { OwnerName = "Alice", InitialBalance = 500m });

            Assert.Equal(500m, result.Balance);
            Assert.Single(result.Transactions);
            Assert.Equal("Credit", result.Transactions[0].Type);
        }

        [Fact]
        public async Task CreateWallet_WithZeroBalance_HasNoTransactions()
        {
            var context = CreateContext();
            var service = new WalletService(context);

            var result = await service.CreateWalletAsync(
                new CreateWalletDto { OwnerName = "Bob", InitialBalance = 0m });

            Assert.Empty(result.Transactions);
        }

        [Fact]
        public async Task GetWalletById_WhenMissing_ReturnsNull()
        {
            var service = new WalletService(CreateContext());

            Assert.Null(await service.GetWalletByIdAsync(999));
        }

        [Fact]
        public async Task Deposit_IncreasesBalance_AndAddsCreditTransaction()
        {
            var context = CreateContext();
            var service = new WalletService(context);
            var wallet = await service.CreateWalletAsync(
                new CreateWalletDto { OwnerName = "Alice", InitialBalance = 100m });

            var ok = await service.DepositAsync(wallet.Id, 50m);

            Assert.True(ok);
            var updated = await service.GetWalletByIdAsync(wallet.Id);
            Assert.Equal(150m, updated!.Balance);
            Assert.Equal(2, updated.Transactions.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Deposit_WithInvalidAmount_ReturnsFalse(int amount)
        {
            var context = CreateContext();
            var service = new WalletService(context);
            var wallet = await service.CreateWalletAsync(
                new CreateWalletDto { OwnerName = "Alice", InitialBalance = 100m });

            Assert.False(await service.DepositAsync(wallet.Id, amount));
        }

        [Fact]
        public async Task Deposit_ToMissingWallet_ReturnsFalse()
        {
            var service = new WalletService(CreateContext());

            Assert.False(await service.DepositAsync(999, 50m));
        }

        [Fact]
        public async Task Withdraw_ReducesBalance_AndAddsDebitTransaction()
        {
            var context = CreateContext();
            var service = new WalletService(context);
            var wallet = await service.CreateWalletAsync(
                new CreateWalletDto { OwnerName = "Alice", InitialBalance = 200m });

            var ok = await service.WithdrawAsync(wallet.Id, 75m);

            Assert.True(ok);
            var updated = await service.GetWalletByIdAsync(wallet.Id);
            Assert.Equal(125m, updated!.Balance);
            Assert.Contains(updated.Transactions, t => t.Type == "Debit" && t.Amount == 75m);
        }

        [Fact]
        public async Task Withdraw_MoreThanBalance_ReturnsFalse_AndKeepsBalance()
        {
            var context = CreateContext();
            var service = new WalletService(context);
            var wallet = await service.CreateWalletAsync(
                new CreateWalletDto { OwnerName = "Alice", InitialBalance = 100m });

            var ok = await service.WithdrawAsync(wallet.Id, 500m);

            Assert.False(ok);
            var unchanged = await service.GetWalletByIdAsync(wallet.Id);
            Assert.Equal(100m, unchanged!.Balance);
            Assert.Single(unchanged.Transactions);
        }
    }
}