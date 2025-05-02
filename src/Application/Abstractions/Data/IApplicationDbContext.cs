using Domain.Aggregates.Bookmarks;
using Domain.Aggregates.Internships;
using Domain.Aggregates.Profiles;
using Domain.Aggregates.Roadmaps;
using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<StudentProfile> StudentProfiles { get; }
    DbSet<StudentExperience> StudentExperiences { get; }
    DbSet<StudentProject> StudentProjects { get; }
    DbSet<CompanyProfile> CompanyProfiles { get; }
    DbSet<Skill> Skills { get; }
    DbSet<StudentSkill> StudentSkills { get; }
    DbSet<Internship> Internships { get; }
    DbSet<Domain.Aggregates.Internships.Application> Applications { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<Roadmap> Roadmaps { get; }
    DbSet<RoadmapSection> RoadmapSections { get; }
    DbSet<RoadmapItem> RoadmapItems { get; }
    DbSet<ResourceProgress> ResourceProgresses { get; }
    DbSet<InternshipBookmark> InternshipBookmarks { get; }
    DbSet<RoadmapBookmark> RoadmapBookmarks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}