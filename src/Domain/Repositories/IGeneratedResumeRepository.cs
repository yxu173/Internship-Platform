using Domain.Aggregates.Resumes;

namespace Domain.Repositories
{
    public interface IGeneratedResumeRepository
    {
        Task<IEnumerable<GeneratedResume>> GetByStudentAndInternshipAsync(Guid studentId, Guid internshipId);
        Task<GeneratedResume> GetLatestByStudentAndInternshipAsync(Guid studentId, Guid internshipId);
        Task<GeneratedResume> GetByIdAsync(Guid id);
        Task<bool> AddAsync(GeneratedResume resume);
        Task<bool> UpdateAsync(GeneratedResume resume);
        Task<bool> DeleteAsync(Guid id);
    }
}
