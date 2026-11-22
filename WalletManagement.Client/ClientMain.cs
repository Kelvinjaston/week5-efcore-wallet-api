namespace WalletManagement.Api.WalletManagement.Client
{
    public class ClientMain
    {
        public static async Task Main(string[] args)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };

            Console.WriteLine("=== Wallet Management Client Starting ===");

            string walletId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
            await GetWalletDetailsAsync(client, walletId);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static async Task GetWalletDetailsAsync(HttpClient httpClient, string walletId)
        {
            try
            {
                Console.WriteLine($"Fetching details for wallet ID: {walletId}...");
                HttpResponseMessage response = await httpClient.GetAsync($"/api/walletqueries/{walletId}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("\n[Success] API Response:");
                    Console.WriteLine(jsonResponse);
                }
                else
                {
                    Console.WriteLine($"\n[Error] Failed with status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"\n[Network Exception] Ensure the API is running: {ex.Message}");
            }
        }
    }
}

