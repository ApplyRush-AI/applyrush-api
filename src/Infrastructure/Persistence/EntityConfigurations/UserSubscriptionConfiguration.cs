using Domain.Entities.Subscriptions.UserSubscriptions;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class UserSubscriptionConfiguration : EntityTypeConfiguration<UserSubscription>
{
    protected override void OnConfigure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<UserSubscription>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.StripeCustomerId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.StripeSubscriptionId)
            .HasMaxLength(256);

        builder.HasIndex(e => e.StripeCustomerId)
            .IsUnique();

        builder.HasIndex(e => e.StripeSubscriptionId)
            .IsUnique()
            .HasFilter("\"StripeSubscriptionId\" IS NOT NULL");
    }
}
