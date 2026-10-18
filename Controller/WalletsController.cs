using Microsoft.AspNetCore.Mvc;
using WalletManagement.Api.DTOs;
using WalletManagement.Api.Services;

namespace WalletManagement.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class WalletsController(IWalletService walletService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WalletResponseDto>>> GetWallets()
    {
        var wallets = await walletService.GetAllWalletsAsync();
        return Ok(wallets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WalletResponseDto>> GetWallet(int id)
    {
        var wallet = await walletService.GetWalletByIdAsync(id);
        if (wallet == null) return NotFound();
        return Ok(wallet);
    }

    [HttpPost]
    public async Task<ActionResult<WalletResponseDto>> CreateWallet([FromBody] CreateWalletDto createWalletDto)
    {
        var createdWallet = await walletService.CreateWalletAsync(createWalletDto);
        return CreatedAtAction(nameof(GetWallet), new { id = createdWallet.Id }, createdWallet);
    }

    [HttpPost("{id:int}/deposit")]
    public async Task<IActionResult> Deposit(int id, [FromBody] TransactionRequestDto request)
    {
        var success = await walletService.DepositAsync(id, request.Amount);
        if (!success) return BadRequest("Deposit failed. Verify wallet ID and amount.");
        return Ok(new { Message = "Deposit successful" });
    }

    [HttpPost("{id:int}/withdraw")]
    public async Task<IActionResult> Withdraw(int id, [FromBody] TransactionRequestDto request)
    {
        var success = await walletService.WithdrawAsync(id, request.Amount);
        if (!success) return BadRequest("Withdrawal failed. Verify wallet ID, amount, or insufficient balance.");
        return Ok(new { Message = "Withdrawal successful" });
    }
}