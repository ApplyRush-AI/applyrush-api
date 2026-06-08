using Domain.Entities.Profiles;
using Domain.Entities.Profiles.EeoDatas;
using Domain.Entities.Profiles.UserProfiles;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class UserProfileConfiguration : EntityTypeConfiguration<UserProfile>
{
    protected override void OnConfigure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<UserProfile>(p => p.UserId);

        builder.HasOne(p => p.EeoData)
            .WithOne(e => e.UserProfile)
            .HasForeignKey<EeoData>(e => e.UserProfileId);
    }
}
