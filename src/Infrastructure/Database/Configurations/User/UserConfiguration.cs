using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.User;

public class UserConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Users.User>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Users.User> builder)
    {
        builder.ToTable("Users");

        builder.Ignore(u => u.PhoneNumber);
        builder.Ignore(u => u.PhoneNumberConfirmed);
        
        builder.HasIndex(u => u.UserName)
            .IsUnique();
        
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.LinkedInUrl)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(u => u.GitHubUrl)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(u => u.ProfileComplete)
            .HasDefaultValue(false);

       
        builder.HasOne(u => u.StudentProfile)
            .WithOne()
            .HasForeignKey<StudentProfile>(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.CompanyProfile)
            .WithOne()
            .HasForeignKey<CompanyProfile>(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}