using Domain.Entities.Jobs.JobListings;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class JobListingConfiguration : EntityTypeConfiguration<JobListing>
{
    protected override void OnConfigure(EntityTypeBuilder<JobListing> builder)
    {
        builder.HasIndex(j => new { j.ExternalId, j.Source }).IsUnique();

        builder.Property(j => j.SalaryMin).HasPrecision(18, 2);
        builder.Property(j => j.SalaryMax).HasPrecision(18, 2);

    }
}
