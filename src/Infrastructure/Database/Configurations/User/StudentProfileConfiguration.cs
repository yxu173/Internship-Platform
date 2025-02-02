using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.User;

public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.ToTable("StudentProfiles");

        builder.Property(sp => sp.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sp => sp.Faculty)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sp => sp.Bio)
            .HasMaxLength(500);

        builder.Property(sp => sp.ProfilePictureUrl)
            .HasMaxLength(255);

        builder.OwnsOne(sp => sp.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(15)
                .IsRequired();
        });

        builder.OwnsOne(sp => sp.GraduationYear, year =>
        {
            year.Property(y => y.Value)
                .HasColumnName("GraduationYear")
                .IsRequired();
        });

      
        builder.Property(sp => sp.University)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(sp => sp.Gender)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();
    }
}