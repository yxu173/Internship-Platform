using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public sealed class ResourceProgressConfiguration : IEntityTypeConfiguration<ResourceProgress>
{
    public void Configure(EntityTypeBuilder<ResourceProgress> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Enrollment)
            .WithMany(x => x.Progress)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(i => new { i.EnrollmentId, i.ItemId }).IsUnique();
    }
}