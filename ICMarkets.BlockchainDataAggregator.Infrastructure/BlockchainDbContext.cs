using ICMarkets.BlockchainDataAggregator.Domain;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.BlockchainDataAggregator.Infrastructure;

public class BlockchainDbContext(DbContextOptions<BlockchainDbContext> options) : DbContext(options)
{
    public DbSet<BlockchainData> BlockchainData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BlockchainData>()
               .HasIndex(b => new { b.Hash, b.Height })
               .IsUnique(); // Ensures unique blockchain records

        modelBuilder.Entity<BlockchainData>()
            .HasIndex(b => b.CreatedAt)
            .HasDatabaseName("IX_BlockchainData_CreatedAt");

        modelBuilder.Entity<BlockchainData>()
            .HasIndex(b => new { b.Name, b.CreatedAt })
            .HasDatabaseName("IX_BlockchainData_Name_CreatedAt");

        modelBuilder.Entity<BlockchainData>()
            .HasIndex(b => b.Hash)
            .IsUnique()
            .HasDatabaseName("IX_BlockchainData_Hash");

        modelBuilder.Entity<BlockchainData>()
            .ToTable("BlockchainData");
    }
    
}
