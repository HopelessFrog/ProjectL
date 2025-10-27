using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext()
    {
    }

    public DbSet<Wallet> Wallets { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!Database.IsInMemory()) modelBuilder.HasPostgresEnum<TransactionType>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseNpgsql("YourConnectionStringHere");
    }
}