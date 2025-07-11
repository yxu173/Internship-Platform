using Application.Abstractions.Data;
using Domain.Aggregates.Bookmarks;
using Domain.Aggregates.Internships;
using Domain.Aggregates.Profiles;
using Domain.Aggregates.Resumes;
using Domain.Aggregates.Roadmaps;
using Domain.Aggregates.Users;
using Domain.ValueObjects;
using Infrastructure.Database.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResourceLink = Domain.Aggregates.Roadmaps.ResourceLink;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options), IApplicationDbContext
{
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<StudentExperience?> StudentExperiences { get; set; }
    public DbSet<StudentProject> StudentProjects { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<StudentSkill> StudentSkills { get; set; }
    public DbSet<Internship> Internships { get; set; }
    public DbSet<Domain.Aggregates.Internships.Application> Applications { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Roadmap> Roadmaps { get; set; }
    public DbSet<RoadmapSection> RoadmapSections { get; set; }
    public DbSet<RoadmapItem> RoadmapItems { get; set; }
    public DbSet<ResourceLink> ResourceLinks { get; set; }
    public DbSet<ResourceProgress> ResourceProgresses { get; set; }
    public DbSet<InternshipBookmark> InternshipBookmarks { get; set; }
    public DbSet<RoadmapBookmark> RoadmapBookmarks { get; set; }
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<GeneratedResume> GeneratedResumes { get; set; }
    
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<QuizOption> QuizOptions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<PhoneNumber>();
        builder.Ignore<Year>();
        builder.Ignore<EgyptianTaxId>();
        builder.Ignore<DateRange>();
        builder.Ignore<Address>();
        builder.Ignore<CompanyAbout>();
        builder.Ignore<Salary>();
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Update(object entity)
    {
        return base.Update(entity);
    }

    public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class
    {
        return base.Add(entity);
    }
}