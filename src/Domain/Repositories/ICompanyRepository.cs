using Domain.Aggregates.Users;

namespace Domain.Repositories;
    public interface ICompanyRepository
    {
        Task<CompanyProfile> GetByIdAsync(Guid id);
        Task<CompanyProfile> GetByEmailAsync(string email);
        Task<CompanyProfile> GetByUsernameAsync(string username);
        Task CreateAsync(CompanyProfile company);
        Task UpdateAsync(CompanyProfile company);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<CompanyProfile>> GetAllAsync();
    }
