using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class RoadmapSectionConfiguration: IEntityTypeConfiguration<RoadmapSection>
{
    public void Configure(EntityTypeBuilder<RoadmapSection> builder)
    {
        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}