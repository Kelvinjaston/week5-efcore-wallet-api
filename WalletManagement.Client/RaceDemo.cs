using System.Linq;
using System.Net.Http.Json;

namespace WalletManagement.Client;

public static class RaceDemo
{
    private const decimal StartingBalance = 100m;
    private const decimal WithdrawAmount = 30m;
    private const int Attempts = 20;

    public static async Task RunAsync(HttpClient client)
    {
        Console.WriteLine("=== Concurrent withdrawal demo ===");

        var created = await client.PostAsJsonAsync("/api/Wallets",
            new { OwnerName = "Race Demo", InitialBalance = StartingBalance });
        created.EnsureSuccessStatusCode();
        var wallet = (await created.Content.ReadFromJsonAsync<WalletDto>())!;

        Console.WriteLine($"Wallet {wallet.Id} created with balance {StartingBalance}.");
        Console.WriteLine($"Sending {Attempts} withdrawals of {WithdrawAmount} at the same time...");

        var responses = await Task.WhenAll(
            Enumerable.Range(0, Attempts).Select(_ =>
                client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/withdraw", new { Amount = WithdrawAmount })));

        var succeeded = responses.Count(r => r.IsSuccessStatusCode);
        var rejected = responses.Count(r => (int)r.StatusCode == 400);
        var errors = responses.Length - succeeded - rejected;

        var final = (await client.GetFromJsonAsync<WalletDto>($"/api/Wallets/{wallet.Id}"))!;
        var debitRows = final.Transactions.Count(t => t.Type == "Debit");

        var expectedBalance = StartingBalance - WithdrawAmount * succeeded;
        var consistent = final.Balance == expectedBalance && final.Balance >= 0 && debitRows == succeeded;

        Console.WriteLine();
        Console.WriteLine($"Succeeded: {succeeded}   Rejected (400): {rejected}   Errors: {errors}");
        Console.WriteLine($"Final balance: {final.Balance} (expected {expectedBalance} for {succeeded} successful withdrawals)");
        Console.WriteLine($"Debit transactions recorded: {debitRows}");
        Console.WriteLine(consistent
            ? "RESULT: PASS - balance and ledger agree, nothing was overdrawn."
            : "RESULT: FAIL - lost updates: the balance does not match the withdrawals that succeeded.");
    }
}