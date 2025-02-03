using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMarkets.BlockchainDataAggregator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blockchains");

            migrationBuilder.CreateTable(
                name: "BlockchainData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<string>(type: "TEXT", nullable: false),
                    LatestUrl = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousHash = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousUrl = table.Column<string>(type: "TEXT", nullable: false),
                    PeerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UnconfirmedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HighFeePerKb = table.Column<int>(type: "INTEGER", nullable: false),
                    MediumFeePerKb = table.Column<int>(type: "INTEGER", nullable: false),
                    LowFeePerKb = table.Column<int>(type: "INTEGER", nullable: false),
                    LastForkHeight = table.Column<int>(type: "INTEGER", nullable: false),
                    LastForkHash = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainData_Hash_Height",
                table: "BlockchainData",
                columns: new[] { "Hash", "Height" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockchainData");

            migrationBuilder.CreateTable(
                name: "Blockchains",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    HighFeePerKb = table.Column<int>(type: "INTEGER", nullable: false),
                    LastForkHash = table.Column<string>(type: "TEXT", nullable: false),
                    LastForkHeight = table.Column<int>(type: "INTEGER", nullable: false),
                    LatestUrl = table.Column<string>(type: "TEXT", nullable: false),
                    LowFeePerKb = table.Column<int>(type: "INTEGER", nullable: false),
                    MediumFeePerKb = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PeerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousHash = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<string>(type: "TEXT", nullable: false),
                    UnconfirmedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blockchains", x => x.Id);
                });
        }
    }
}
