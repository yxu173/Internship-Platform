using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.User;

public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.ToTable("CompanyProfiles");

        builder.HasKey(e => e.Id);

        builder.Property(cp => cp.CompanyName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(cp => cp.Industry)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(cp => cp.WebsiteUrl)
            .HasMaxLength(255);

        builder.Property(cp => cp.Description)
            .HasMaxLength(1000);

        builder.Property(cp => cp.Size)
            .HasConversion<string>()
            .HasMaxLength(20);


        builder.OwnsOne(cp => cp.TaxId, taxId =>
        {
            taxId.Property(t => t.Value)
                .HasColumnName("TaxId")
                .HasMaxLength(20)
                .IsRequired();
        });
        builder.OwnsOne(cp => cp.Address, address =>
        {
            address.Property(t => t.Governorate)
                .HasColumnName("Governorate")
                .HasConversion<string>()
                .IsRequired();
            address.Property(t => t.City)
                .IsRequired();
        });
        builder.OwnsOne(cp => cp.About, about =>
        {
            about.Property(a => a.Mission)
                .HasColumnName("Mission")
                .HasMaxLength(1000);
            about.Property(a => a.Vision)
                .HasColumnName("Vision")
                .HasMaxLength(1000);
            about.Property(a => a.About)
                .HasColumnName("About")
                .HasMaxLength(1000);
        });

        builder.HasMany(cp => cp.Internships)
            .WithOne(i => i.CompanyProfile)
            .HasForeignKey(i => i.CompanyProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}