using Microsoft.EntityFrameworkCore;
using StockScreener.Domain.Entities;

namespace StockScreener.Infrastructure.Persistence;

public class StockScreenerDbContext : DbContext
{
    public StockScreenerDbContext(DbContextOptions<StockScreenerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<DailyCandle> DailyCandlesDb { get; set; }
    public DbSet<ScreenRun> ScreenRuns { get; set; }
    public DbSet<ScreenResult> ScreenResults { get; set; }
    public DbSet<NewsArticle> NewsArticles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Stock configuration
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(s => s.CompanyName).IsRequired().HasMaxLength(255);
            entity.Property(s => s.Exchange).IsRequired().HasMaxLength(10);
            entity.Property(s => s.MarketCapUSD).HasPrecision(18, 2);
            entity.Property(s => s.AverageDailyVolumeUSD).HasPrecision(18, 2);

            entity.HasMany(s => s.DailyCandlesLast60)
                .WithOne(c => c.Stock)
                .HasForeignKey(c => c.StockId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.ScreenResults)
                .WithOne(r => r.Stock)
                .HasForeignKey(r => r.StockId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(s => s.Ticker).IsUnique();
        });

        // DailyCandle configuration
        modelBuilder.Entity<DailyCandle>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Open).HasPrecision(18, 4);
            entity.Property(d => d.High).HasPrecision(18, 4);
            entity.Property(d => d.Low).HasPrecision(18, 4);
            entity.Property(d => d.Close).HasPrecision(18, 4);

            entity.HasIndex(d => new { d.StockId, d.Date }).IsUnique();
        });

        // ScreenRun configuration
        modelBuilder.Entity<ScreenRun>(entity =>
        {
            entity.HasKey(sr => sr.Id);

            entity.HasMany(sr => sr.Results)
                .WithOne(r => r.ScreenRun)
                .HasForeignKey(r => r.ScreenRunId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ScreenResult configuration
        modelBuilder.Entity<ScreenResult>(entity =>
        {
            entity.HasKey(sr => sr.Id);
            entity.Property(sr => sr.TotalScore).HasPrecision(5, 2);
            entity.Property(sr => sr.VolatilityScore).HasPrecision(5, 2);
            entity.Property(sr => sr.DrawdownScore).HasPrecision(5, 2);
            entity.Property(sr => sr.ExtremeScore).HasPrecision(5, 2);
            entity.Property(sr => sr.LiquidityScore).HasPrecision(5, 2);
            entity.Property(sr => sr.MetricsExplanation).IsRequired().HasColumnType("TEXT");

            entity.HasIndex(sr => new { sr.ScreenRunId, sr.Rank });
        });

        // NewsArticle configuration
        modelBuilder.Entity<NewsArticle>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Ticker).HasMaxLength(10);
            entity.Property(n => n.Title).IsRequired().HasMaxLength(500);
            entity.Property(n => n.Source).IsRequired().HasMaxLength(100);
            entity.Property(n => n.Category).IsRequired().HasMaxLength(50);

            entity.HasIndex(n => new { n.Ticker, n.PublishedAt });
            entity.HasIndex(n => n.Category);
        });
    }
}
