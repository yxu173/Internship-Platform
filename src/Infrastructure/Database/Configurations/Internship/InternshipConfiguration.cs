using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Internship;

public sealed class InternshipConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Internships.Internship>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Internships.Internship> builder)
    {
        builder.ToTable("Internships");

        builder.Property(i => i.Type)
            .HasConversion<string>();

        builder.OwnsOne(i => i.Duration, d =>
        {
            d.Property(dr => dr.StartDate).HasColumnName("StartDate");
            d.Property(dr => dr.EndDate).HasColumnName("EndDate");
        });

        builder.Property(i => i.WorkingModel)
            .HasConversion<string>();

        builder.OwnsOne(i => i.Salary, s =>
        {
            s.Property(s => s.Amount).HasColumnName("Salary");
            s.Property(s => s.Currency).HasColumnName("Currency");
        });

        builder.HasOne<CompanyProfile>(i => i.CompanyProfile)
            .WithMany()
            .HasForeignKey(i => i.CompanyProfileId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(i => i.IsActive).HasDefaultValue(true);
    }
}