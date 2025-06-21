using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
{
    public void Configure(EntityTypeBuilder<QuizQuestion> builder)
    {
        builder.ToTable("QuizQuestions");
        
        builder.HasKey(q => q.Id);
        
        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(q => q.Points)
            .IsRequired();
            
        // Options relationship
        builder.HasMany(q => q.Options)
            .WithOne()
            .HasForeignKey("QuestionId")
            .OnDelete(DeleteBehavior.Cascade);
    }
} 