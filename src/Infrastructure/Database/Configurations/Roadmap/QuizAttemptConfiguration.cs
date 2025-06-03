using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.EnrollmentId)
            .IsRequired();
            
        builder.Property(a => a.QuizId)
            .IsRequired();
            
        builder.Property(a => a.Score)
            .IsRequired();
            
        builder.Property(a => a.Passed)
            .IsRequired();
            
        builder.Property(a => a.CreatedAt)
            .IsRequired();
            
        builder.Property(a => a.ModifiedAt);
            
        builder.OwnsMany(a => a.Answers, answersBuilder =>
        {
            answersBuilder.ToTable("QuizAnswers");
            
            answersBuilder.WithOwner().HasForeignKey("QuizAttemptId");
            
            answersBuilder.Property(answer => answer.QuestionId)
                .IsRequired();
                
            answersBuilder.Property(answer => answer.SelectedOptionId)
                .IsRequired();
                
            answersBuilder.Property(answer => answer.IsCorrect)
                .IsRequired();
        });

        builder.HasOne<Enrollment>()
            .WithMany(e => e.QuizAttempts)
            .HasForeignKey(a => a.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 