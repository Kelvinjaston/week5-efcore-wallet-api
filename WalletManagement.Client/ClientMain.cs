using System.Net.Http.Json;
using System.Text.Json;

namespace WalletManagement.Client;

public record TransactionDto(int Id, int WalletId, string Type, decimal Amount, DateTime CreatedAt);
public record WalletDto(int Id, string OwnerName, decimal Balance, DateTime CreatedAt, List<TransactionDto> Transactions);

public class ClientMain
{
    private static readonly JsonSerializerOptions PrettyJson = new() { WriteIndented = true };

    public static async Task Main(string[] args)
    {
        var baseUrl = Array.Find(args, a => a.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                      ?? "http://localhost:5181";
        using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

        Console.WriteLine($"=== Wallet Management Client -> {baseUrl} ===");

        try
        {
            if (Array.IndexOf(args, "race") >= 0)
            {
                await RaceDemo.RunAsync(client);
                return;
            }

            var wallet = await CreateWalletAsync(client, "Ada Lovelace", 1000m);
            if (wallet is null) return;

            await SendAmountAsync(client, wallet.Id, "deposit", 250m);
            await SendAmountAsync(client, wallet.Id, "withdraw", 100m);
            await SendAmountAsync(client, wallet.Id, "withdraw", 1_000_000m); // expected to fail: insufficient balance

            await PrintGetAsync(client, $"/api/Wallets/{wallet.Id}", "Wallet details");
            await PrintGetAsync(client, "/api/Wallets", "All wallets");
            await PrintGetAsync(client, "/api/WalletQueries/high-value?minBalance=500", "High-value wallets");
            await PrintGetAsync(client, "/api/WalletQueries/liquidity", "Total system liquidity");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[Network error] Is the API running? {ex.Message}");
        }
    }
    private static async Task<WalletDto?> CreateWalletAsync(HttpClient client, string owner, decimal initialBalance)
    {
        Console.WriteLine($"\nPOST /api/Wallets ({owner}, {initialBalance})");
        var response = await client.PostAsJsonAsync("/api/Wallets",
            new { OwnerName = owner, InitialBalance = initialBalance });

        if (!response.IsSuccessStatusCode)
        {
            await PrintFailureAsync(response);
            return null;
        }

        var wallet = await response.Content.ReadFromJsonAsync<WalletDto>();
        Console.WriteLine($"[{(int)response.StatusCode}] Created wallet {wallet!.Id} with balance {wallet.Balance}");
        return wallet;
    }

    private static async Task SendAmountAsync(HttpClient client, int walletId, string action, decimal amount)
    {
        Console.WriteLine($"\nPOST /api/Wallets/{walletId}/{action} (amount {amount})");
        var response = await client.PostAsJsonAsync($"/api/Wallets/{walletId}/{action}", new { Amount = amount });

        if (response.IsSuccessStatusCode)
            Console.WriteLine($"[{(int)response.StatusCode}] {await response.Content.ReadAsStringAsync()}");
        else
            await PrintFailureAsync(response);
    }
    private static async Task PrintGetAsync(HttpClient client, string url, string title)
    {
        Console.WriteLine($"\nGET {url}  ({title})");
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            await PrintFailureAsync(response);
            return;
        }

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Console.WriteLine($"[{(int)response.StatusCode}]");
        Console.WriteLine(JsonSerializer.Serialize(doc.RootElement, PrettyJson));
    }

    private static async Task PrintFailureAsync(HttpResponseMessage response)
    {
        Console.WriteLine($"[{(int)response.StatusCode} {response.ReasonPhrase}] {await response.Content.ReadAsStringAsync()}");
    }
}