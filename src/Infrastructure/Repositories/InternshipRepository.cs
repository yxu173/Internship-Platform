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

    public async Task<Internship?> GetByIdAsync(Guid id)
    {
        return await _context.Internships
            .Include(i => i.Applications)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Internship>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Internships
            .Where(i => i.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<List<Internship>> GetActiveInternshipsAsync()
    {
        return await _context.Internships
            .Where(i => i.IsActive)
            .ToListAsync();
    }

    public async Task<List<Internship>> SearchAsync(string searchTerm)
    {
        return await _context.Internships
            .Where(i => EF.Functions.ILike(i.Title, $"%{searchTerm}%") ||
                        EF.Functions.ILike(i.Description, $"%{searchTerm}%"))
            .ToListAsync();
    }

    public async Task AddAsync(Internship internship)
    {
        await _context.Internships.AddAsync(internship);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Internship internship)
    {
        _context.Update(internship);
        await _context.SaveChangesAsync();
    }
}