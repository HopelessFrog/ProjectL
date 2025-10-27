using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public sealed class TransactionEntityConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(transaction => transaction.Id);
        
        builder.Property(t => t.Type)
            .IsRequired();

        builder.OwnsOne(t => t.Amount, amountBuilder =>
        {
            amountBuilder.Property(a => a.Value)
                .HasColumnName(nameof(Transaction.Amount))
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.OwnsOne(t => t.Description, descriptionBuilder =>
        {
            descriptionBuilder.Property(d => d.Value)
                .HasColumnName(nameof(Transaction.Description))
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.HasOne(transaction => transaction.Wallet)
            .WithMany(wallet => wallet.Transactions)
            .HasForeignKey(transaction => transaction.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(transaction => new { transaction.WalletId, transaction.Date, transaction.Type });
    }
}