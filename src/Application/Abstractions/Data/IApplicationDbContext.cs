﻿using Domain.Aggregates.Internships;
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
    DbSet<Internship> Internships { get; }
    DbSet<Domain.Aggregates.Internships.Application> Applications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}