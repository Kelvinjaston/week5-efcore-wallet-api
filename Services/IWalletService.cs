using WalletManagement.Api.DTOs;

namespace WalletManagement.Api.Services;

public interface IWalletService
{
    Task<IEnumerable<WalletResponseDto>> GetAllWalletsAsync();
    Task<WalletResponseDto?> GetWalletByIdAsync(int id);
    Task<WalletResponseDto> CreateWalletAsync(CreateWalletDto createWalletDto);
    Task<bool> DepositAsync(int walletId, decimal amount);
    Task<bool> WithdrawAsync(int walletId, decimal amount);
}