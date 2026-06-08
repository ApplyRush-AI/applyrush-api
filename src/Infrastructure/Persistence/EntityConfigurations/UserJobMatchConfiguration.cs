using Domain.Entities.Jobs.UserJobMatches;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class UserJobMatchConfiguration : EntityTypeConfiguration<UserJobMatch>
{
    protected override void OnConfigure(EntityTypeBuilder<UserJobMatch> builder)
    {
        builder.HasIndex(m => new { m.UserId, m.JobId }).IsUnique();

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Job)
            .WithMany(j => j.UserMatches)
            .HasForeignKey(m => m.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(m => m.OverallScore).HasPrecision(5, 2);
        builder.Property(m => m.ExperienceScore).HasPrecision(5, 2);
        builder.Property(m => m.SkillScore).HasPrecision(5, 2);
        builder.Property(m => m.TitleScore).HasPrecision(5, 2);
        builder.Property(m => m.IndustryScore).HasPrecision(5, 2);
    }
}
