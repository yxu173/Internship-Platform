using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> CreateAsync(Guid userId,
     string companyName,
      string taxId,
       string governorate,
        string industry)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            return Result.Failure<bool>(UserErrors.UserNotFound);

        if (user.ProfileComplete == true)
            return Result.Failure<bool>(CompanyErrors.AlreadyRegistered);

        user.CreateCompanyProfile(companyName, taxId,
         governorate, industry);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Result.Success(true);
    }

    public Task<CompanyProfile> GetByIdAsync(Guid id)
    {
        return _context.CompanyProfiles.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(CompanyProfile company)
    {
        _context.CompanyProfiles.Update(company);
        await _context.SaveChangesAsync();
    }
}