using Domain.Aggregates.Bookmarks;
using SharedKernel;

namespace Domain.Repositories;

public interface IRoadmapBookmarkRepository
{
    Task<RoadmapBookmark?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoadmapBookmark?> FindByUserAndRoadmapIdAsync(Guid studentId, Guid roadmapId, CancellationToken cancellationToken = default);
    Task<List<RoadmapBookmark>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task AddAsync(RoadmapBookmark bookmark, CancellationToken cancellationToken = default);
    void Remove(RoadmapBookmark bookmark);
} 