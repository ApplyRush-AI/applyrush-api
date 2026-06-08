using Domain.Entities.Tailoring.ResumeTailorings;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class ResumeTailoringConfiguration : EntityTypeConfiguration<ResumeTailoring>
{
    protected override void OnConfigure(EntityTypeBuilder<ResumeTailoring> builder)
    {
        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Resume)
            .WithMany(r => r.Tailorings)
            .HasForeignKey(t => t.ResumeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Job)
            .WithMany()
            .HasForeignKey(t => t.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.TailoredContent)
            .IsRequired();

        builder.Property(t => t.ScoreBefore)
            .HasPrecision(5, 2);

        builder.Property(t => t.ScoreAfter)
            .HasPrecision(5, 2);
    }
}
