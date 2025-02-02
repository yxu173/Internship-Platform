using Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<StudentProfile> StudentProfiles { get; }
    DbSet<CompanyProfile> CompanyProfiles { get; }
    DbSet<Skill> Skills { get; }
    DbSet<StudentSkill> StudentSkills { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}