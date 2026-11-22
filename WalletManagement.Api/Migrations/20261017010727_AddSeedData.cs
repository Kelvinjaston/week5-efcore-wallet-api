using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WalletManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "id", "Balance", "CreatedAt", "OwnerName" },
                values: new object[,]
                {
                    { 1, 50000.00m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kelvin Jaston" },
                    { 2, 150000.00m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "John Doe" }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "CreatedAt", "Type", "WalletId" },
                values: new object[,]
                {
                    { 1, 10000.00m, new DateTime(2026, 1, 2, 10, 30, 0, 0, DateTimeKind.Utc), "Credit", 1 },
                    { 2, 5000.00m, new DateTime(2026, 1, 3, 14, 15, 0, 0, DateTimeKind.Utc), "Debit", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
