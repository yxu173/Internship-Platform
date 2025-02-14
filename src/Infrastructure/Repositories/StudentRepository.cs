using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result> CreateAsync(Guid userId,
        string fullName,
        string university,
        string faculty,
        int graduationYear,
        int enrollmentYear,
        int age,
        string gender,
        string phoneNumber)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return Result.Failure<bool>(UserErrors.UserNotFound);

        if (user.ProfileComplete == true)
            return Result.Failure<bool>(StudentErrors.AlreadyExists);

        var student = user.CreateStudentProfile(
            fullName,
            university,
            faculty,
            graduationYear,
            enrollmentYear,
            age,
            gender,
            phoneNumber);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<StudentProfile> GetByIdAsync(Guid id)
    {
        return (await _context.StudentProfiles.FirstOrDefaultAsync(s => s.UserId == id))!;
    }


    public async Task UpdateAsync(StudentProfile student)
    {
        _context.StudentProfiles.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<StudentSkill>> GetStudentSkillsAsync(Guid studentId)
    {
        return await _context.StudentSkills
            .Where(ss => ss.StudentId == studentId)
            .Include(ss => ss.Skill)
            .ToListAsync();
    }

    public async Task<StudentProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.StudentProfiles
            .Include(s => s.Skills)
            .ThenInclude(ss => ss.Skill)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<Result> CreateStudentExperienceAsync(Guid studentId,
        string jobTitle,
        string companyName,
        DateTime startDate,
        DateTime endDate)
    {
        var studentExperience = StudentExperience.Create(
            studentId,
            jobTitle,
            companyName,
            startDate,
            endDate);
        if (studentExperience.IsFailure)
            return Result.Failure<bool>(studentExperience.Error);
        await _context.StudentExperiences.AddAsync(studentExperience.Value);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> RemoveStudentExperienceAsync(Guid experienceId)
    {
        var studentExperience = await _context.StudentExperiences.FirstOrDefaultAsync(se => se.Id == experienceId);
        if (studentExperience is null)
            return Result.Failure<bool>(StudentErrors.ExperienceNotFound);
        _context.StudentExperiences.Remove(studentExperience);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<IReadOnlyList<StudentExperience>> GetAllStudentExperiences(Guid studentId)
    {
        return await _context.StudentExperiences
            .Where(se => se.StudentProfileId == studentId)
            .ToListAsync();
    }

    public async Task<Result> CreateStudentProjectAsync(Guid studentId, string projectName, string description,
        string projectUrl)
    {
        var studentProject = StudentProject.Create(studentId, projectName, 
            description, projectUrl);
        if (studentProject.IsFailure)
            return Result.Failure<bool>(studentProject.Error);
        await _context.StudentProjects.AddAsync(studentProject.Value);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
}