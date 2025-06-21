using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");
        
        builder.HasKey(q => q.Id);
        
        builder.Property(q => q.PassingScore)
            .IsRequired();
            
        builder.Property(q => q.IsRequired)
            .IsRequired();

        builder.HasOne(q => q.Section)
            .WithOne(s => s.Quiz)
            .HasForeignKey<Quiz>(q => q.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Questions)
            .WithOne()
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 