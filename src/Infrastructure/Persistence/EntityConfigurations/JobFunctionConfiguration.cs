using Domain.Entities.JobFunctions;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class JobFunctionConfiguration : EntityTypeConfiguration<JobFunction>
{
    protected override void OnConfigure(EntityTypeBuilder<JobFunction> builder)
    {
        builder.HasOne(j => j.Parent)
            .WithMany(j => j.Children)
            .HasForeignKey(j => j.ParentId)
            .IsRequired(false);
    }
}
