using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class EnrollmentSectionProgressConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.OwnsMany(e => e.SectionProgress, progressBuilder =>
        {
            progressBuilder.ToTable("SectionProgress");
            
            progressBuilder.WithOwner().HasForeignKey("EnrollmentId");
            
            progressBuilder.Property(p => p.SectionId)
                .IsRequired();
                
            progressBuilder.Property(p => p.QuizPassed)
                .IsRequired();
                
            progressBuilder.HasKey(nameof(SectionProgress.SectionId), "EnrollmentId");
        });
    }
} 