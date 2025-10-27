using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class WalletEntityConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(wallet => wallet.Id);

        builder.OwnsOne(w => w.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName(nameof(Wallet.Name))
                .HasMaxLength(30)
                .IsRequired();
            
            name.HasIndex(n => n.Value);
        });

        builder.OwnsOne(w => w.Currency, currency =>
        {
            currency.Property(c => c.Value)
                .HasColumnName(nameof(Wallet.Currency))
                .HasMaxLength(3)
                .IsRequired();

            currency.HasIndex(c => c.Value);
        });
        
        builder.OwnsOne(w => w.InitialBalance, balance =>
        {
            balance.Property(b => b.Value)
                .HasColumnName(nameof(Wallet.InitialBalance))
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });
        
        builder.HasMany(w => w.Transactions)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}