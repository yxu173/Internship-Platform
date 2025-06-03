using Domain.Aggregates.Roadmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class QuizOptionConfiguration : IEntityTypeConfiguration<QuizOption>
{
    public void Configure(EntityTypeBuilder<QuizOption> builder)
    {
        builder.ToTable("QuizOptions");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Text)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(o => o.IsCorrect)
            .IsRequired();
            
        builder.Property<Guid>("QuestionId")
            .IsRequired();
    }
} 