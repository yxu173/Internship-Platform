using Domain.Aggregates.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.User;

public class StudentProjectConfiguration : IEntityTypeConfiguration<StudentProject>
{
    public void Configure(EntityTypeBuilder<StudentProject> builder)
    {
        builder.ToTable("StudentProjects");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ProjectName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.ProjectUrl)
            .HasMaxLength(200);
    }
}