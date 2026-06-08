using Domain.Entities.Tailoring.ResumeAnalyses;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class ResumeAnalysisConfiguration : EntityTypeConfiguration<ResumeAnalysis>
{
    protected override void OnConfigure(EntityTypeBuilder<ResumeAnalysis> builder)
    {
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.Issues)
            .IsRequired();
    }
}
