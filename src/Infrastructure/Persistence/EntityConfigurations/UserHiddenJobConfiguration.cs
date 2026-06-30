using Domain.Entities.Jobs.UserHiddenJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class UserHiddenJobConfiguration : IEntityTypeConfiguration<UserHiddenJob>
{
    public void Configure(EntityTypeBuilder<UserHiddenJob> builder)
    {
        builder.HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.Job)
            .WithMany()
            .HasForeignKey(h => h.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => new { h.UserId, h.JobId }).IsUnique();
    }
}
