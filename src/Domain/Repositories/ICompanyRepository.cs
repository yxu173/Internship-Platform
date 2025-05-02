using System.Linq.Expressions;
using System.Reflection.Metadata;
using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using SharedKernel;

namespace Domain.Repositories;
public interface ICompanyRepository
{
    Task<CompanyProfile?> GetCompanyByIdAsync(Guid id);
    Task<Result<CompanyProfile>> CreateAsync(Guid userId,
     string companyName,
      string taxId,
       string governorate,
        string city,
        string street,
        string industry);
    Task<Result<bool>> UpdateBasicInfoAsync(Guid userId, string name, string industry, string description, string websiteUrl,
        string companySize);
    Task<Result<bool>> UpdateCompanyLogo(Guid userId, string logoUrl);
    Task<Result<bool>> UpdateCompanyAbout(Guid userId, string about, string mission, string vision);
    Task<Result<T?>> GetByUserIdAsync<T>(Guid userId, Expression<Func<CompanyProfile, T>> selector);
    Task<Result<T?>> GetByCompanyProfileWithInternships<T>(Guid userId, Expression<Func<CompanyProfile, T>> selector);

}
