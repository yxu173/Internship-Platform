using Domain.Aggregates.Bookmarks;
using SharedKernel;

namespace Domain.Repositories;

public interface IInternshipBookmarkRepository
{
    Task<InternshipBookmark?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InternshipBookmark?> FindByUserAndInternshipIdAsync(Guid studentId, Guid internshipId, CancellationToken cancellationToken = default);
    Task<List<InternshipBookmark>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task AddAsync(InternshipBookmark bookmark, CancellationToken cancellationToken = default);
    Task Remove(InternshipBookmark bookmark);
} 