using Domain.Aggregates.Roadmaps;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Roadmap;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.StudentId)
            .IsRequired();

        builder.Property(x => x.RoadmapId)
            .IsRequired();

        builder.Property(x => x.EnrolledAt)
            .IsRequired();

        builder.Property(x => x.PaymentStatus)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v));

        builder.Property(x => x.PaymentProviderTransactionId)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.AmountPaid)
            .HasPrecision(10, 2)
            .IsRequired(false);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Roadmap)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.RoadmapId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 