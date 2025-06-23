using System.Linq;
using Domain.Aggregates.Internships;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class InternshipRepository : IInternshipRepository
{
    private readonly ApplicationDbContext _context;

    public InternshipRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Internship?> GetByIdAsync(Guid id, bool includeApplications = false)
    {
        var query = _context.Internships.AsQueryable();

        if (includeApplications)
        {
            query = query.Include(i => i.Applications);
        }

        return await query.FirstOrDefaultAsync(i => i.Id == id);
    }

    public Task<Internship?> GetById(Guid id)
    {
        return _context.Internships.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Internship> GetByInternshipIdWithCompanyAsync(Guid internshipId)
    {
        return await _context.Internships
            .Include(i => i.CompanyProfile)
            .FirstOrDefaultAsync(i => i.Id == internshipId);
    }

    public async Task<IReadOnlyList<Internship>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Internships
            .Where(i => i.CompanyProfileId == companyId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Internship>> GetCompanyIntenshipsByUserIdAsync(Guid userId)
    {
        return await _context.Internships
            .Where(i => i.CompanyProfile.UserId == userId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Internship>> GetActiveInternshipsAsync()
    {
        return await _context.Internships
            .Where(i => i.IsActive && i.ApplicationDeadline > DateTime.UtcNow)
            .Include(i => i.CompanyProfile)
            .Include(i => i.Applications)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Internship>> GetRecentInternshipsAsync(int count)
    {
        return await _context.Internships
            .Where(i => i.IsActive && i.ApplicationDeadline > DateTime.UtcNow)
            .Include(i => i.CompanyProfile)
            .Include(i => i.Applications)
            .OrderByDescending(i => i.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddAsync(Internship internship)
    {
        await _context.Internships.AddAsync(internship);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Internship internship)
    {
        _context.Internships.Update(internship);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Internship internship)
    {
        _context.Internships.Remove(internship);
        await _context.SaveChangesAsync();
    }

    public async Task<Domain.Aggregates.Internships.Application?> GetApplicationByIdAsync(Guid applicationId)
    {
        return await _context.Applications
            .Include(a => a.StudentProfile)
            .FirstOrDefaultAsync(a => a.Id == applicationId);
    }

    public async Task<IReadOnlyList<Domain.Aggregates.Internships.Application>> GetApplicationsByInternshipIdAsync(
        Guid internshipId)
    {
        return await _context.Applications
            .Where(a => a.InternshipId == internshipId)
            .Include(a => a.StudentProfile)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Domain.Aggregates.Internships.Application>> GetApplicationsByStudentIdAsync(
        Guid studentProfileId)
    {
        return await _context.Applications
            .Where(a => a.StudentProfileId == studentProfileId)
            .Include(a => a.Internship)
            .ThenInclude(i => i.CompanyProfile)
            .ToListAsync();
    }

    public async Task AddApplicationAsync(Domain.Aggregates.Internships.Application application)
    {
        await _context.Applications.AddAsync(application);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveApplication(Domain.Aggregates.Internships.Application application)
    {
        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateApplicationAsync(Domain.Aggregates.Internships.Application application)
    {
        _context.Applications.Update(application);
        await _context.SaveChangesAsync();
    }

    public async Task<SearchResult<Internship>> SearchInternshipsAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        searchTerm = searchTerm?.ToLower() ?? string.Empty;
        
        var query = _context.Internships
            .Include(i => i.CompanyProfile)
            .Include(i => i.Applications)
            .Where(i => i.IsActive &&
                   (string.IsNullOrEmpty(searchTerm) ||
                    i.Title.ToLower().Contains(searchTerm) ||
                    i.Requirements.ToLower().Contains(searchTerm) ||
                    i.KeyResponsibilities.ToLower().Contains(searchTerm) ||
                    i.CompanyProfile.CompanyName.ToLower().Contains(searchTerm) ||
                    i.Type.ToString().ToLower().Contains(searchTerm) ||
                    i.WorkingModel.ToString().ToLower().Contains(searchTerm) ||
                    i.CompanyProfile.Industry.ToLower().Contains(searchTerm)))
            .OrderByDescending(i => i.CreatedAt);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new SearchResult<Internship>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<Domain.Aggregates.Internships.Application>> GetAcceptedApplicationsByInternshipIdAsync(
        Guid internshipId)
    {
        return await _context.Applications
            .Where(a => a.InternshipId == internshipId && a.Status == Domain.Enums.ApplicationStatus.Accepted)
            .Include(a => a.StudentProfile)
            .ToListAsync();
    }
}