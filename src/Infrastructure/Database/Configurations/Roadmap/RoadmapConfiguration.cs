using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class RoadmapConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Roadmaps.Roadmap>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Roadmaps.Roadmap> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.Property(x => x.Technology)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.IsPremium)
            .IsRequired();
        
        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(x => x.CompanyId)
            .IsRequired();

        builder.HasMany(x => x.Sections)
            .WithOne()
            .HasForeignKey(s => s.RoadmapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Sections)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Enrollments)
            .WithOne(e => e.Roadmap)
            .HasForeignKey(e => e.RoadmapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Enrollments)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(x => x.Company)
               .WithMany()
               .HasForeignKey(x => x.CompanyId)
               .IsRequired();

        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.Technology);
    }
}