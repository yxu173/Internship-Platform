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

    public async Task<InternshipBookmark?> FindByUserAndInternshipIdAsync(Guid studentId, Guid internshipId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InternshipBookmarks
            .FirstOrDefaultAsync(b => b.StudentId == studentId && b.InternshipId == internshipId, cancellationToken);
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