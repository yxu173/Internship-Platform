using System.Reflection.Metadata;
using Domain.Aggregates.Users;
using SharedKernel;

namespace Domain.Repositories;
public interface ICompanyRepository
{
    Task<CompanyProfile> GetByIdAsync(Guid id);
    Task<Result<bool>> CreateAsync(Guid userId,
     string companyName,
      string taxId,
       string governorate,
        string industry);
    Task UpdateAsync(CompanyProfile company);
}
