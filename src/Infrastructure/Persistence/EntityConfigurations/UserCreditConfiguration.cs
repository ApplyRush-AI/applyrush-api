using Domain.Entities.Subscriptions.UserCredits;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class UserCreditConfiguration : EntityTypeConfiguration<UserCredit>
{
    protected override void OnConfigure(EntityTypeBuilder<UserCredit> builder)
    {
        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<UserCredit>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
