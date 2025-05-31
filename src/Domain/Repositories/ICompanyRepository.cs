using System.Linq.Expressions;
using Domain.Aggregates.Profiles;
using SharedKernel;

namespace Domain.Repositories;

public interface ICompanyRepository
{
    Task<CompanyProfile?> GetCompanyByIdAsync(Guid id);
    Task<CompanyProfile?> GetByCompanyIdAsync(Guid companyId);

    Task<Result<CompanyProfile>> CreateAsync(Guid userId,
        string companyName,
        string taxId,
        string governorate,
        string city,
        string industry);

    Task Update(CompanyProfile company);

    Task<Result<bool>> UpdateBasicInfoAsync(Guid userId, string industry, string websiteUrl,
        string companySize, string yearOfEstablishment);

    Task<Result<bool>> UpdateCompanyLogo(Guid userId, string logoUrl);
    Task<Result<bool>> UpdateCompanyAbout(Guid userId, string about, string mission, string vision);
    Task<Result<T?>> GetByUserIdAsync<T>(Guid userId, Expression<Func<CompanyProfile, T>> selector);
    Task<Result<T?>> GetByCompanyProfileWithInternships<T>(Guid userId, Expression<Func<CompanyProfile, T>> selector);
    
    Task<SearchResult<CompanyProfile>> SearchCompaniesAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
}