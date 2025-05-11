using Domain.Aggregates.Roadmaps;

namespace Domain.Repositories;

public interface IRoadmapRepository
{
    // Roadmap Operations
    Task<Roadmap?> GetByIdAsync(Guid id, bool includeSections = false);
    Task<IReadOnlyList<Roadmap>> GetByTechnologyAsync(string technology);
    Task<IReadOnlyList<Roadmap>> GetByCompanyIdAsync(Guid companyId);
    Task<IReadOnlyList<Roadmap>> GetPublicRoadmapsAsync();
    Task AddAsync(Roadmap roadmap);
    Task Update(Roadmap roadmap);
    Task Delete(Roadmap roadmap);

    // Section Operations
    Task<RoadmapSection?> GetSectionByIdAsync(Guid sectionId, bool includeItems = false);
    Task AddSectionAsync(Guid roadmapId, RoadmapSection section);
    Task UpdateSectionAsync(RoadmapSection section);
    Task DeleteSectionAsync(RoadmapSection section);

    // Enrollment Operations
    Task<Enrollment?> GetEnrollmentAsync(Guid studentId, Guid roadmapId);
    Task AddEnrollmentAsync(Enrollment enrollment);
    Task UpdateEnrollmentAsync(Enrollment enrollment);
    
    // Progress Tracking
    Task<ResourceProgress?> GetProgressAsync(Guid enrollmentId, Guid itemId);
    Task TrackProgressAsync(ResourceProgress progress);

    Task<IReadOnlyList<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId, bool includeRoadmap = false);
    
    Task<SearchResult<Roadmap>> SearchRoadmapsAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResourceProgress>> GetProgressByStudentIdAsync(Guid studentId);

    Task<IReadOnlyList<ResourceProgress>> GetProgressByEnrollmentIdAsync(Guid enrollmentId);

    Task<IReadOnlyList<Enrollment>> GetEnrollmentsByRoadmapIdsAsync(IEnumerable<Guid> roadmapIds);

    Task<IReadOnlyList<ResourceProgress>> GetProgressByEnrollmentIdsAsync(IEnumerable<Guid> enrollmentIds);
}