using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMarkets.BlockchainDataAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addeIndexesToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BlockchainData_CreatedAt",
                table: "BlockchainData",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainData_Hash",
                table: "BlockchainData",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainData_Name_CreatedAt",
                table: "BlockchainData",
                columns: new[] { "Name", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlockchainData_CreatedAt",
                table: "BlockchainData");

            migrationBuilder.DropIndex(
                name: "IX_BlockchainData_Hash",
                table: "BlockchainData");

            migrationBuilder.DropIndex(
                name: "IX_BlockchainData_Name_CreatedAt",
                table: "BlockchainData");
        }
    }
}
