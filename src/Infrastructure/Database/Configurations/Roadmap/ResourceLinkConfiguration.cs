using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class ResourceLinkConfiguration : IEntityTypeConfiguration<ResourceLink>
{
    public void Configure(EntityTypeBuilder<ResourceLink> builder)
    {
        builder.ToTable("ResourceLink");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .UseIdentityColumn();
            
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();
            
        // Configure the relationship
        builder.HasOne<RoadmapItem>()
            .WithMany(x => x.Resources)
            .HasForeignKey("RoadmapItemId")
            .IsRequired();
    }
}
