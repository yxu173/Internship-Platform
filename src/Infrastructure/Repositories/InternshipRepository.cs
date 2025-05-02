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

    public async Task<IReadOnlyList<Internship>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Internships
        .Where(i => i.CompanyProfileId == companyId)
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

    public async Task<IReadOnlyList<Domain.Aggregates.Internships.Application>> GetApplicationsByInternshipIdAsync(Guid internshipId)
    {
        return await _context.Applications
            .Where(a => a.InternshipId == internshipId)
            .Include(a => a.StudentProfile)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Domain.Aggregates.Internships.Application>> GetApplicationsByStudentIdAsync(Guid studentProfileId)
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
}
