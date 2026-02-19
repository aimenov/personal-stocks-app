using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockScreener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewsArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScreenRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RunDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalStocksScanned = table.Column<int>(type: "INTEGER", nullable: false),
                    StocksWithValidScore = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Exchange = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    MarketCapUSD = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    AverageDailyVolumeUSD = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyCandlesDb",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StockId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    High = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Close = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Volume = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyCandlesDb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyCandlesDb_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScreenResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScreenRunId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StockId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalScore = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    VolatilityScore = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    DrawdownScore = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    ExtremeScore = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    LiquidityScore = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    MetricsExplanation = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenResults_ScreenRuns_ScreenRunId",
                        column: x => x.ScreenRunId,
                        principalTable: "ScreenRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScreenResults_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyCandlesDb_StockId_Date",
                table: "DailyCandlesDb",
                columns: new[] { "StockId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_Category",
                table: "NewsArticles",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_Ticker_PublishedAt",
                table: "NewsArticles",
                columns: new[] { "Ticker", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenResults_ScreenRunId_Rank",
                table: "ScreenResults",
                columns: new[] { "ScreenRunId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_ScreenResults_StockId",
                table: "ScreenResults",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Ticker",
                table: "Stocks",
                column: "Ticker",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyCandlesDb");

            migrationBuilder.DropTable(
                name: "NewsArticles");

            migrationBuilder.DropTable(
                name: "ScreenResults");

            migrationBuilder.DropTable(
                name: "ScreenRuns");

            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}
