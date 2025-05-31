using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using SharedKernel;

namespace Domain.Repositories;

public interface IStudentRepository
{
    Task<StudentProfile> GetByIdAsync(Guid id);
    Task<StudentProfile> GetFullProfileByUserId(Guid id);

    Task<Result<StudentProfile>> CreateAsync(Guid userId,
        string fullName,
        string university,
        string faculty,
        int graduationYear,
        int enrollmentYear,
        int age,
        string gender,
        string phoneNumber,
        string? bio,
        string? profilePictureUrl);

    Task<IReadOnlyList<StudentSkill>> GetStudentSkillsAsync(Guid studentId);
    Task UpdateAsync(StudentProfile student);
    Task<StudentProfile?> GetByUserIdAsync(Guid userId);

    Task<Result> CreateStudentExperienceAsync(Guid studentId,
        string jobTitle,
        string companyName,
        DateTime startDate,
        DateTime endDate);

    Task<StudentExperience?> GetStudentExperienceById(Guid studentExperienceId);

    Task UpdateStudentExperience(StudentExperience studentExperience);
    Task<Result> RemoveStudentExperienceAsync(Guid experienceId);
    Task<IReadOnlyList<StudentExperience?>> GetAllStudentExperiences(Guid studentId);

    Task<Result> CreateStudentProjectAsync(
        Guid studentId,
        string projectName,
        string description,
        string projectUrl);

    Task<IReadOnlyList<StudentProject>> GetAllStudentProjects(Guid studentId);

    Task<Result> UpdateStudentProjectAsync(Guid studentId, Guid projectId, string projectName, string description,
        string projectUrl);

    Task<Result> RemoveStudentProjectAsync(Guid studentId, Guid projectId);
    Task<StudentProfile?> GetCompleteStudentProfile(Guid studentProfileId);
}