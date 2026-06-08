using Domain.Entities.Resumes.Resume;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class ResumeConfiguration : EntityTypeConfiguration<Resume>
{
    protected override void OnConfigure(EntityTypeBuilder<Resume> builder)
    {
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
    }
}
