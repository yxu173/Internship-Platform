using Domain.Aggregates.Roadmaps;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
                .ThenInclude(s => s.Items)
                .ThenInclude(i => i.Resources)
                .Include(x => x.Sections)
                .ThenInclude(s => s.Quiz);
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
        //return await _context.Roadmaps.AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(Roadmap roadmap)
    {
        await _context.Roadmaps.AddAsync(roadmap);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Roadmap roadmap)
    {
        var existingRoadmap = await _context.Roadmaps
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roadmap.Id);
            
        if (existingRoadmap == null)
        {
            _context.Roadmaps.Add(roadmap);
        }
        else
        {
            _context.Roadmaps.Update(roadmap);
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Roadmap roadmap)
    {
        _context.Roadmaps.Remove(roadmap);
        await _context.SaveChangesAsync();
    }

    public async Task<RoadmapSection?> GetSectionByIdAsync(Guid sectionId, bool includeItems = false, bool includeQuiz = false)
    {
        var query = _context.RoadmapSections.AsQueryable();

        if (includeItems)
        {
            query = query.Include(s => s.Items)
                  .ThenInclude(i => i.Resources);
        }

        if (includeQuiz)
        {
            query = query.Include(s => s.Quiz)
                .ThenInclude(q => q.Questions)
                .ThenInclude(q => q.Options);
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
        return await _context.Enrollments
            .Include(e => e.SectionProgress)
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.RoadmapId == roadmapId);
    }

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEnrollmentAsync(Enrollment enrollment)
    {
        // Check if we have any new section progress records that need to be added
        var existingSectionProgressIds = await _context.Entry(enrollment)
            .Collection(e => e.SectionProgress)
            .Query()
            .Select(sp => sp.SectionId)
            .ToListAsync();
            
        var modifiedEntries = _context.ChangeTracker
            .Entries<SectionProgress>()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        // For any missing section progress, manually add it
        foreach (var sectionProgress in enrollment.SectionProgress)
        {
            if (!existingSectionProgressIds.Contains(sectionProgress.SectionId))
            {
                // Manually add a new SectionProgress record to the database
                var sql = $@"
                    INSERT INTO ""SectionProgress"" (""SectionId"", ""EnrollmentId"", ""QuizPassed"")
                    VALUES (@sectionId, @enrollmentId, @quizPassed)";
                
                await _context.Database.ExecuteSqlRawAsync(sql, 
                    new NpgsqlParameter("@sectionId", sectionProgress.SectionId),
                    new NpgsqlParameter("@enrollmentId", enrollment.Id),
                    new NpgsqlParameter("@quizPassed", sectionProgress.QuizPassed));
            }
        }
        
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
    
    public async Task<SearchResult<Roadmap>> SearchRoadmapsAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        searchTerm = searchTerm?.ToLower() ?? string.Empty;
        
        var query = _context.Roadmaps
            .Include(r => r.Company)
            .Include(r => r.Sections)
                .ThenInclude(s => s.Items)
            .Where(r => string.IsNullOrEmpty(searchTerm) ||
                   r.Title.ToLower().Contains(searchTerm) ||
                   r.Description.ToLower().Contains(searchTerm) ||
                   r.Technology.ToLower().Contains(searchTerm) ||
                   r.Company.CompanyName.ToLower().Contains(searchTerm) ||
                   r.Sections.Any(s => s.Title.ToLower().Contains(searchTerm)) ||
                   r.Sections.Any(s => s.Items.Any(i => i.Title.ToLower().Contains(searchTerm))))
            .OrderByDescending(r => r.CreatedAt);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new SearchResult<Roadmap>(items, totalCount, page, pageSize);
    }
}