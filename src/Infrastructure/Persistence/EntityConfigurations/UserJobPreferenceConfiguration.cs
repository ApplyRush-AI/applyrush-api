using Domain.Entities.Settings;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class UserJobPreferenceConfiguration : EntityTypeConfiguration<UserJobPreference>
{
    protected override void OnConfigure(EntityTypeBuilder<UserJobPreference> builder)
    {
        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<UserJobPreference>(p => p.UserId);
    }
}
