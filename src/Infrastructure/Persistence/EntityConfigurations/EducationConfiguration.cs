using Domain.Entities.Profiles;
using Domain.Entities.Profiles.Educations;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class EducationConfiguration : EntityTypeConfiguration<Education>
{
    protected override void OnConfigure(EntityTypeBuilder<Education> builder)
    {
        // (5,2) rather than (3,2): the 10.0 scale needs to store 10.00, and the "Other" scale allows
        // larger values such as a percentage GPA. Max storable is 999.99.
        builder.Property(e => e.Gpa)
            .HasPrecision(5, 2);
    }
}
