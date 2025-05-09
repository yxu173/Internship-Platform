using Domain.Aggregates.Roadmaps;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RoadmapRepository : IRoadmapRepository
{
    private readonly ApplicationDbContext _context;

    public RoadmapRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Roadmap?> GetByIdAsync(Guid id, bool includeSections = false)
    {
        var query = _context.Roadmaps.AsNoTracking();

        if (includeSections)
        {
            query = query.Include(x => x.Sections)
                .ThenInclude(s => s.Items);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Roadmap>> GetByTechnologyAsync(string technology)
    {
        return await _context.Roadmaps.AsNoTracking().Where(x => x.Technology == technology).ToListAsync();
    }

    public async Task<IReadOnlyList<Roadmap>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Roadmaps.AsNoTracking().Where(x => x.CompanyId == companyId).ToListAsync();
    }

    public async Task<IReadOnlyList<Roadmap>> GetPublicRoadmapsAsync()
    {
        return await _context.Roadmaps.AsNoTracking().Where(x => !x.IsPremium).ToListAsync();
    }

    public async Task AddAsync(Roadmap roadmap)
    {
        await _context.Roadmaps.AddAsync(roadmap);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Roadmap roadmap)
    {
        _context.Entry(roadmap).State = EntityState.Modified;

        foreach (var section in roadmap.Sections)
        {
            if (section.Id == Guid.Empty)
            {
                _context.Entry(section).State = EntityState.Added;
            }
            else
            {
                _context.Entry(section).State = EntityState.Modified;
            }

            foreach (var item in section.Items)
            {
                if (item.Id == Guid.Empty)
                {
                    _context.Entry(item).State = EntityState.Added;
                }
                else
                {
                    _context.Entry(item).State = EntityState.Modified;
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task Delete(Roadmap roadmap)
    {
        _context.Roadmaps.Remove(roadmap);
        await _context.SaveChangesAsync();
    }

    public async Task<RoadmapSection?> GetSectionByIdAsync(Guid sectionId, bool includeItems = false)
    {
        var query = _context.RoadmapSections.AsQueryable();

        if (includeItems)
        {
            query = query.Include(s => s.Items);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == sectionId);
    }

    public async Task AddSectionAsync(Guid roadmapId, RoadmapSection section)
    {
        await _context.RoadmapSections.AddAsync(section);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSectionAsync(RoadmapSection section)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSectionAsync(RoadmapSection section)
    {
        _context.RoadmapSections.Remove(section);
        await _context.SaveChangesAsync();
    }

    public async Task<Enrollment?> GetEnrollmentAsync(Guid studentId, Guid roadmapId)
    {
        return await _context.Enrollments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.RoadmapId == roadmapId);
    }

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEnrollmentAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task<ResourceProgress?> GetProgressAsync(Guid enrollmentId, Guid itemId)
    {
        return await _context.ResourceProgresses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EnrollmentId == enrollmentId && x.ItemId == itemId);
    }

    public async Task TrackProgressAsync(ResourceProgress progress)
    {
        await _context.ResourceProgresses.AddAsync(progress);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId,
        bool includeRoadmap = false)
    {
        var query = _context.Enrollments.AsNoTracking().Where(e => e.StudentId == studentId);

        if (includeRoadmap)
        {
            query = query.Include(e => e.Roadmap);
        }

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<ResourceProgress>> GetProgressByStudentIdAsync(Guid studentId)
    {
        var enrollmentIds = await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.Id)
            .ToListAsync();

        if (!enrollmentIds.Any())
        {
            return new List<ResourceProgress>();
        }

        return await _context.ResourceProgresses.AsNoTracking()
            .Where(rp => enrollmentIds.Contains(rp.EnrollmentId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ResourceProgress>> GetProgressByEnrollmentIdAsync(Guid enrollmentId)
    {
        return await _context.ResourceProgresses.AsNoTracking()
            .Where(rp => rp.EnrollmentId == enrollmentId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Enrollment>> GetEnrollmentsByRoadmapIdsAsync(IEnumerable<Guid> roadmapIds)
    {
        if (roadmapIds == null || !roadmapIds.Any()) return new List<Enrollment>();

        return await _context.Enrollments.AsNoTracking()
            .Where(e => roadmapIds.Contains(e.RoadmapId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ResourceProgress>> GetProgressByEnrollmentIdsAsync(IEnumerable<Guid> enrollmentIds)
    {
        if (enrollmentIds == null || !enrollmentIds.Any()) return new List<ResourceProgress>();

        return await _context.ResourceProgresses.AsNoTracking()
            .Where(rp => enrollmentIds.Contains(rp.EnrollmentId))
            .ToListAsync();
    }
}