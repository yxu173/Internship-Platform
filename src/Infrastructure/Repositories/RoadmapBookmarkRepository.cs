using Domain.Aggregates.Bookmarks;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class RoadmapBookmarkRepository : IRoadmapBookmarkRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoadmapBookmarkRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<Guid, int>> GetBookmarkCountsForRoadmapsAsync(List<Guid> roadmapIds, CancellationToken cancellationToken = default)
    {
        if (roadmapIds == null || !roadmapIds.Any())
            return new Dictionary<Guid, int>();
            
        var bookmarkCounts = await _dbContext.RoadmapBookmarks
            .Where(b => roadmapIds.Contains(b.RoadmapId))
            .GroupBy(b => b.RoadmapId)
            .Select(g => new { RoadmapId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoadmapId, x => x.Count, cancellationToken);
            
        return bookmarkCounts;
    }

    public async Task AddAsync(RoadmapBookmark bookmark, CancellationToken cancellationToken = default)
    {
        await _dbContext.RoadmapBookmarks.AddAsync(bookmark, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RoadmapBookmark?> FindByUserAndRoadmapIdAsync(Guid studentId, Guid roadmapId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoadmapBookmarks
            .FirstOrDefaultAsync(b => b.StudentId == studentId && b.RoadmapId == roadmapId, cancellationToken);
    }

    public async Task<RoadmapBookmark?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoadmapBookmarks.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
    }

    public async Task<List<RoadmapBookmark>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoadmapBookmarks
            .Where(b => b.StudentId == studentId)
            .Include(b => b.Roadmap) 
            .ThenInclude(r => r.Company)
            .ToListAsync(cancellationToken);
    }

    public void Remove(RoadmapBookmark bookmark)
    {
        _dbContext.RoadmapBookmarks.Remove(bookmark);
        _dbContext.SaveChanges();
    }
} 