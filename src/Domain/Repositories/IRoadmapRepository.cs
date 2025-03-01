using Domain.Aggregates.Roadmaps;

namespace Domain.Repositories;

public interface IRoadmapRepository
{
    // Roadmap Operations
    Task<Roadmap?> GetByIdAsync(Guid id, bool includeSections = false);
    Task<IReadOnlyList<Roadmap>> GetByTechnologyAsync(string technology);
    Task<IReadOnlyList<Roadmap>> GetByCompanyIdAsync(Guid companyId);
    Task<IReadOnlyList<Roadmap>> GetPublicRoadmapsAsync(int page = 1, int pageSize = 20);
    Task AddAsync(Roadmap roadmap);
    Task Update(Roadmap roadmap);
    Task Delete(Roadmap roadmap);

    // Section Operations
    Task<RoadmapSection?> GetSectionByIdAsync(Guid sectionId);
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
}