using Domain.Entities.Profiles;
using Domain.Entities.Profiles.Educations;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class EducationConfiguration : EntityTypeConfiguration<Education>
{
    protected override void OnConfigure(EntityTypeBuilder<Education> builder)
    {
        builder.Property(e => e.Gpa)
            .HasPrecision(3, 2);
    }
}
