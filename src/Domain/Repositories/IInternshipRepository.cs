using Domain.Aggregates.Internships;

namespace Domain.Repositories;

public interface IInternshipRepository
{
    Task<Internship?> GetByIdAsync(Guid id);
    Task<List<Internship>> GetByCompanyIdAsync(Guid companyId);
    Task<List<Internship>> GetActiveInternshipsAsync();
    Task<List<Internship>> SearchAsync(string searchTerm);
    Task AddAsync(Internship internship);
    Task UpdateAsync(Internship internship);
}