using Domain.Entities.Jobs.UserSavedJobs;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class UserSavedJobConfiguration : EntityTypeConfiguration<UserSavedJob>
{
    protected override void OnConfigure(EntityTypeBuilder<UserSavedJob> builder)
    {
        builder.HasIndex(s => new { s.UserId, s.JobId }).IsUnique();

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Job)
            .WithMany(j => j.SavedByUsers)
            .HasForeignKey(s => s.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
