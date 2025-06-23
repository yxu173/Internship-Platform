using Domain.Aggregates.Internships;

namespace Domain.Repositories;

public interface IInternshipRepository
{
    Task<Internship?> GetByIdAsync(Guid id, bool includeApplications = false);
    Task<Internship?> GetById(Guid id);
    Task<Internship> GetByInternshipIdWithCompanyAsync(Guid internshipId);
    Task<IReadOnlyList<Internship>> GetByCompanyIdAsync(Guid companyId);
    Task<IReadOnlyList<Internship>> GetCompanyIntenshipsByUserIdAsync(Guid userId);

    Task<IReadOnlyList<Internship>> GetActiveInternshipsAsync();
    Task<IReadOnlyList<Internship>> GetRecentInternshipsAsync(int count);
    Task AddAsync(Internship internship);
    Task Update(Internship internship);
    Task Delete(Internship internship);
    
    Task<Application?> GetApplicationByIdAsync(Guid applicationId);
    Task<IReadOnlyList<Application>> GetApplicationsByInternshipIdAsync(Guid internshipId);
    Task<IReadOnlyList<Application>> GetAcceptedApplicationsByInternshipIdAsync(Guid internshipId);
    Task<IReadOnlyList<Application>> GetApplicationsByStudentIdAsync(Guid studentProfileId);
    Task AddApplicationAsync(Application application);
    Task RemoveApplication(Application application);
    Task UpdateApplicationAsync(Application application);
    
    Task<SearchResult<Internship>> SearchInternshipsAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
}