using Domain.Entities.Extension.ExtensionSessions;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public sealed class ExtensionSessionConfiguration : EntityTypeConfiguration<ExtensionSession>
{
    protected override void OnConfigure(EntityTypeBuilder<ExtensionSession> builder)
    {
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.JobUrl)
            .IsRequired()
            .HasMaxLength(2048);
    }
}
