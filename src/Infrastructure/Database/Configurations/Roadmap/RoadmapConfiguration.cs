using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class RoadmapConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Roadmaps.Roadmap>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Roadmaps.Roadmap> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(r => r.Technology)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(r => r.IsPremium)
            .IsRequired();
        builder.Property(r => r.Price)
            .HasPrecision(18, 2);
        builder.Property(r => r.CompanyId)
            .IsRequired();
        
        builder.HasMany(r => r.Sections)
            .WithOne()
            .HasForeignKey(s => s.RoadmapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Enrollments)
            .WithOne(x => x.Roadmap)
            .HasForeignKey(x => x.RoadmapId);

        builder.HasIndex(r => r.CompanyId);
        builder.HasIndex(r => r.Technology);
    }
}