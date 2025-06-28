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

    public async Task<CompanyProfile?> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.CompanyProfiles.FirstOrDefaultAsync(x => x.Id == companyId);
    }

    public async Task<Result<CompanyProfile>> CreateAsync(Guid userId,
        string companyName,
        string taxId,
        string governorate,
        string city,
        string industry)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            return Result.Failure<CompanyProfile>(UserErrors.UserNotFound);

        if (user.ProfileComplete == true)
            return Result.Failure<CompanyProfile>(CompanyErrors.AlreadyRegistered);

        user.CreateCompanyProfile(companyName, taxId,
            governorate,
            city,
            industry);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Result.Success(user.CompanyProfile);
    }

    public async Task Update(CompanyProfile company)
    {
        _context.CompanyProfiles.Update(company);
        await _context.SaveChangesAsync();
    }


    public async Task<Result<bool>> UpdateBasicInfoAsync(Guid userId,
        string industry,
        string websiteUrl,
        string companySize,
        string yearOfEstablishment)
    {
        var company = await GetCompanyByIdAsync(userId);
        if (company == null)
            return Result.Failure<bool>(CompanyErrors.ProfileNotFound);
        company.UpdateDetails(industry, websiteUrl, companySize.ToString(),yearOfEstablishment);
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
        var companyProfile = await _context.CompanyProfiles
            .Where(cp => cp.UserId == userId)
            .FirstOrDefaultAsync();

        if (companyProfile == null)
            return Result.Failure<T?>(CompanyErrors.ProfileNotFound);

        var result = selector.Compile()(companyProfile);
        return Result.Success(result);
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
    
    public async Task<SearchResult<CompanyProfile>> SearchCompaniesAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        searchTerm = searchTerm?.ToLower() ?? string.Empty;
        
        var query = _context.CompanyProfiles
            .Include(c => c.Internships)
            .Where(c => string.IsNullOrEmpty(searchTerm) ||
                   c.CompanyName.ToLower().Contains(searchTerm) ||
                   (c.Industry != null && c.Industry.ToLower().Contains(searchTerm)) ||
                   (c.About != null && c.About.About != null && c.About.About.ToLower().Contains(searchTerm)) ||
                   (c.About != null && c.About.Mission != null && c.About.Mission.ToLower().Contains(searchTerm)) ||
                   (c.About != null && c.About.Vision != null && c.About.Vision.ToLower().Contains(searchTerm)) ||
                   (c.Address != null && c.Address.City != null && c.Address.City.ToLower().Contains(searchTerm)) ||
                   (c.Address != null && c.Address.Governorate.ToString().ToLower().Contains(searchTerm)))
            .OrderBy(c => c.CompanyName);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new SearchResult<CompanyProfile>(items, totalCount, page, pageSize);
    }
}