using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class RoadmapItemConfiguration : IEntityTypeConfiguration<RoadmapItem>
{
    public void Configure(EntityTypeBuilder<RoadmapItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();
        builder.Property(x => x.Order)
            .IsRequired();

        builder.OwnsMany(x => x.Resources, r =>
        {
            r.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(100);
            r.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(200);
            r.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired();
        });
    }
}