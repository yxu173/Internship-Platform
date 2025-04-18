using System.Linq.Expressions;
using Domain.Aggregates.Profiles;
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
        string city,
        string street,
        string industry)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            return Result.Failure<bool>(UserErrors.UserNotFound);

        if (user.ProfileComplete == true)
            return Result.Failure<bool>(CompanyErrors.AlreadyRegistered);

        user.CreateCompanyProfile(companyName, taxId,
            governorate,
            city,
            street,
            industry);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Result.Success(true);
    }

    public async Task<Result<bool>> UpdateBasicInfoAsync(Guid userId,
        string name,
        string industry,
        string description,
        string websiteUrl,
        string companySize)
    {
        var company = await GetCompanyByIdAsync(userId);
        if (company == null)
            return Result.Failure<bool>(CompanyErrors.ProfileNotFound);
        company.UpdateDetails(name, industry, websiteUrl, description, companySize.ToString());
        await UpdateAsync(company);
        return Result.Success(true);
    }

    public async Task<Result<bool>> UpdateCompanyLogo(Guid userId, string logoUrl)
    {
        var company = await GetCompanyByIdAsync(userId);
        if (company == null)
            return Result.Failure<bool>(CompanyErrors.ProfileNotFound);
        company.UpdateLogo(logoUrl);
        await UpdateAsync(company);
        return Result.Success(true);
    }

    public async Task<Result<bool>> UpdateCompanyAbout(Guid userId, string about, string mission, string vision)
    {
        var company = await GetCompanyByIdAsync(userId);
        if (company == null)
            return Result.Failure<bool>(CompanyErrors.ProfileNotFound);
        company.About.Update(about, mission, vision);
        await UpdateAsync(company);
        return Result.Success(true);
    }

    public async Task<Result<T?>> GetByUserIdAsync<T>(Guid userId, Expression<Func<CompanyProfile, T>> selector)
    {
        return Result.Success(await _context.CompanyProfiles
            .Where(cp => cp.UserId == userId)
            .Select(selector)
            .FirstOrDefaultAsync());
    }

    public async Task<CompanyProfile?> GetCompanyByIdAsync(Guid id)
    {
        return await _context.CompanyProfiles.FirstOrDefaultAsync(x => x.UserId == id);
    }

    public async Task UpdateAsync(CompanyProfile company)
    {
        _context.CompanyProfiles.Update(company);
        await _context.SaveChangesAsync();
    }

    public async Task<Result<T?>> GetByCompanyProfileWithInternships<T>(Guid userId, Expression<Func<CompanyProfile, T>> selector)
    {
        return Result.Success(await _context.CompanyProfiles
            .Where(cp => cp.UserId == userId)
            .Include(x => x.Internships)
            .Select(selector)
            .FirstOrDefaultAsync());
    }
}