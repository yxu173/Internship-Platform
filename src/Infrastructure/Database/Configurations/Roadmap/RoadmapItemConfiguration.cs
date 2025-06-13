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
        builder.Property(x => x.Order)
            .IsRequired();

        builder.HasMany(x => x.Resources)
               .WithOne()
               .HasForeignKey("RoadmapItemId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Resources)  
               .HasField("_resourceLinks")  
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}