using Domain.Entities.Subscriptions.CreditTransactions;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class CreditTransactionConfiguration : EntityTypeConfiguration<CreditTransaction>
{
    protected override void OnConfigure(EntityTypeBuilder<CreditTransaction> builder)
    {
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.ReferenceType)
            .HasMaxLength(100);
    }
}
