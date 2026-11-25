using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Balance",
                table: "Wallets",
                column: "Balance");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Amount",
                table: "Transactions",
                column: "Amount",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId_CreatedAt",
                table: "Transactions",
                columns: new[] { "WalletId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_Balance",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Amount",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_WalletId_CreatedAt",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions",
                column: "WalletId");
        }
    }
}
