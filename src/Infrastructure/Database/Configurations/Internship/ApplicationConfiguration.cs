using Domain.Aggregates.Users;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Internship;
public sealed class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Internships.Application>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Internships.Application> builder)
    {
        builder.ToTable("Applications");

        builder.Property(a => a.Status)
            .HasConversion<string>();

        builder.HasOne(a => a.Internship)
        .WithMany(i => i.Applications)
        .HasForeignKey(a => a.InternshipId)
        .IsRequired();

        builder.HasOne(a => a.StudentProfile)
            .WithMany()
            .HasForeignKey(a => a.StudentProfileId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(a => a.InternshipId)
            .HasColumnName("InternshipId");

        builder.Property(a => a.StudentProfileId)
            .HasColumnName("StudentProfileId");



        builder.Property(a => a.ResumeUrl).HasMaxLength(500);
        builder.Property(a => a.StudentProfileId).IsRequired();
        builder.Property(a => a.AppliedAt).IsRequired();
        builder.Property(a => a.DecisionDate);
    }
}
