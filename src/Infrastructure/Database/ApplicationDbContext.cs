using Application.Abstractions.Data;
using Domain.Aggregates.Internships;
using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options), IApplicationDbContext
{


    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<StudentExperience> StudentExperiences { get; set; }
    public DbSet<StudentProject> StudentProjects { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<StudentSkill> StudentSkills { get; set; }
    public DbSet<Internship> Internships { get; set; }
    public DbSet<Domain.Aggregates.Internships.Application> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<PhoneNumber>();
        builder.Ignore<Year>();
        builder.Ignore<EgyptianTaxId>();
        builder.Ignore<DateRange>();
        builder.Ignore<Address>();
        builder.Ignore<CompanyAbout>();
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}