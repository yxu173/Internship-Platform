using Domain.Aggregates.Bookmarks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class InternshipBookmarkConfiguration : IEntityTypeConfiguration<InternshipBookmark>
{
    public void Configure(EntityTypeBuilder<InternshipBookmark> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.StudentId).IsRequired();
        builder.Property(b => b.InternshipId).IsRequired();

        builder.HasOne(b => b.Student)
               .WithMany()
               .HasForeignKey(b => b.StudentId)
               .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(b => b.Internship)
               .WithMany()
               .HasForeignKey(b => b.InternshipId)
               .OnDelete(DeleteBehavior.Cascade); 

        builder.HasIndex(b => new { b.StudentId, b.InternshipId }).IsUnique();

        builder.ToTable("InternshipBookmarks");
    }
} 