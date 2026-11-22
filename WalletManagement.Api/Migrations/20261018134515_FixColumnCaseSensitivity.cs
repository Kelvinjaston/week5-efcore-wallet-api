using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixColumnCaseSensitivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Wallets",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Wallets",
                newName: "id");
        }
    }
}
