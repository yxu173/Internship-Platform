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
            

        builder.OwnsOne(cp => cp.TaxId, taxId =>
        {
            taxId.Property(t => t.Value)
                .HasColumnName("TaxId")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(cp => cp.Size)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(cp => cp.Governorate)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }
}