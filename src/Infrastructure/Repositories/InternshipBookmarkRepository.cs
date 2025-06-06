using Domain.Aggregates.Bookmarks;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class InternshipBookmarkRepository : IInternshipBookmarkRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InternshipBookmarkRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<InternshipBookmark>> GetByUserIdAsync(Guid UserId,
        CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.StudentProfiles.FirstOrDefaultAsync(
            x => x.UserId == UserId
            , cancellationToken);
        return await _dbContext.InternshipBookmarks
            .Where(b => b.StudentId == student.Id)
            .Include(b => b.Internship)
            .ThenInclude(i => i.CompanyProfile)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetBookmarkCountsForInternshipsAsync(List<Guid> internshipIds, CancellationToken cancellationToken = default)
    {
        if (internshipIds == null || !internshipIds.Any())
            return new Dictionary<Guid, int>();
            
        var bookmarkCounts = await _dbContext.InternshipBookmarks
            .Where(b => internshipIds.Contains(b.InternshipId))
            .GroupBy(b => b.InternshipId)
            .Select(g => new { InternshipId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.InternshipId, x => x.Count, cancellationToken);
            
        return bookmarkCounts;
    }

    public async Task AddAsync(InternshipBookmark bookmark, CancellationToken cancellationToken = default)
    {
        await _dbContext.InternshipBookmarks.AddAsync(bookmark, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task Remove(InternshipBookmark bookmark)
    {
        _dbContext.InternshipBookmarks.Remove(bookmark);
        return _dbContext.SaveChangesAsync();
    }

    public async Task<InternshipBookmark?> FindByStudentAndInternshipIdAsync(Guid studentId, Guid internshipId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InternshipBookmarks
            .FirstOrDefaultAsync(b => b.StudentId == studentId && b.InternshipId == internshipId, cancellationToken);
    }

    public async Task<InternshipBookmark?> FindByUserAndInternshipIdAsync(Guid UserId, Guid internshipId, CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.StudentProfiles
            .FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken);
        var bookmark = await FindByStudentAndInternshipIdAsync(student.Id, internshipId, cancellationToken);
        return bookmark;
    }

    public async Task<InternshipBookmark?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InternshipBookmarks.FindAsync(new object?[] { id },
            cancellationToken: cancellationToken);
    }

    public async Task<List<InternshipBookmark>> GetByStudentIdAsync(Guid studentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InternshipBookmarks
            .Where(b => b.StudentId == studentId)
            .Include(b => b.Internship)
            .ThenInclude(i => i.CompanyProfile)
            .ToListAsync(cancellationToken);
    }

    public async Task Remove(InternshipBookmark bookmark, CancellationToken cancellationToken = default)
    {
        _dbContext.InternshipBookmarks.Remove(bookmark);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}