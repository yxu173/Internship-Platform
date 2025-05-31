using Domain.Aggregates.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.User;

public class StudentExperienceConfiguration : IEntityTypeConfiguration<StudentExperience>
{
    public void Configure(EntityTypeBuilder<StudentExperience> builder)
    {
        builder.ToTable("StudentExperiences");
        builder.Property(x => x.JobTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CompanyName)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(x => x.DateRange, d =>
        {
            d.Property(dd => dd.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            d.Property(dd => dd.EndDate)
                .HasColumnName("EndDate");
        });
    }
}