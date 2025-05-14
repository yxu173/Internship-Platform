using Domain.Aggregates.Resumes;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GeneratedResumeRepository : IGeneratedResumeRepository
{
    private readonly ApplicationDbContext _context;

    public GeneratedResumeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GeneratedResume>> GetByStudentAndInternshipAsync(Guid studentId, Guid internshipId)
    {
        return await _context.GeneratedResumes
            .Where(r => r.StudentId == studentId && r.InternshipId == internshipId)
            .ToListAsync();
    }

    public async Task<GeneratedResume> GetLatestByStudentAndInternshipAsync(Guid studentId, Guid internshipId)
    {
        return await _context.GeneratedResumes
            .Where(r => r.StudentId == studentId &&
                        r.InternshipId == internshipId &&
                        r.IsLatest)
            .FirstOrDefaultAsync();
    }

    public async Task<GeneratedResume> GetByIdAsync(Guid id)
    {
        return await _context.GeneratedResumes.FindAsync(id);
    }

    public async Task<bool> AddAsync(GeneratedResume resume)
    {
        await _context.GeneratedResumes.AddAsync(resume);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(GeneratedResume resume)
    {
        _context.GeneratedResumes.Update(resume);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var resume = await GetByIdAsync(id);
        if (resume == null) return false;
        
        _context.GeneratedResumes.Remove(resume);
        await _context.SaveChangesAsync();
        return true;
    }
}