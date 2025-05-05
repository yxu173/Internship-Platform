using Domain.Aggregates.Internships;

namespace Domain.Repositories;

public interface IInternshipRepository
{
    Task<Internship?> GetByIdAsync(Guid id, bool includeApplications = false);
    Task<Internship?> GetById(Guid id);
    Task<IReadOnlyList<Internship>> GetByCompanyIdAsync(Guid companyId);
    Task AddAsync(Internship internship);
    Task Update(Internship internship);
    Task Delete(Internship internship);
    
    Task<Application?> GetApplicationByIdAsync(Guid applicationId);
    Task<IReadOnlyList<Application>> GetApplicationsByInternshipIdAsync(Guid internshipId);
    Task<IReadOnlyList<Application>> GetApplicationsByStudentIdAsync(Guid studentProfileId);
    Task AddApplicationAsync(Application application);
    Task RemoveApplication(Application application);
    Task UpdateApplicationAsync(Application application);
}