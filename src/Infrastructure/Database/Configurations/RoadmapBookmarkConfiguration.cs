using Domain.Aggregates.Bookmarks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class RoadmapBookmarkConfiguration : IEntityTypeConfiguration<RoadmapBookmark>
{
    public void Configure(EntityTypeBuilder<RoadmapBookmark> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.StudentId).IsRequired();
        builder.Property(b => b.RoadmapId).IsRequired();

        builder.HasOne(b => b.Student)
               .WithMany()
               .HasForeignKey(b => b.StudentId)
               .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(b => b.Roadmap)
               .WithMany()
               .HasForeignKey(b => b.RoadmapId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => new { b.StudentId, b.RoadmapId }).IsUnique();

        builder.ToTable("RoadmapBookmarks");
    }
} 