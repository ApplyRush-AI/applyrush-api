using Domain.Entities.Jobs.JobListings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class JobListingJobFunctionConfiguration : IEntityTypeConfiguration<JobListingJobFunction>
{
    public void Configure(EntityTypeBuilder<JobListingJobFunction> builder)
    {
        builder.HasOne(jf => jf.JobListing)
            .WithMany(j => j.JobFunctions)
            .HasForeignKey(jf => jf.JobListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(jf => jf.JobFunction)
            .WithMany()
            .HasForeignKey(jf => jf.JobFunctionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(jf => new { jf.JobListingId, jf.JobFunctionId }).IsUnique();
    }
}
