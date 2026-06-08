using Domain.Entities.Settings;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class UserNotificationPreferenceConfiguration : EntityTypeConfiguration<UserNotificationPreference>
{
    protected override void OnConfigure(EntityTypeBuilder<UserNotificationPreference> builder)
    {
        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<UserNotificationPreference>(p => p.UserId);
    }
}
