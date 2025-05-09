using Domain.Aggregates.Roadmaps;
using Domain.Aggregates.Users;
using Domain.Common;
using Domain.DomainErrors;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Profiles;

public sealed class StudentProfile : BaseAuditableEntity
{
    private StudentProfile()
    {
    }

    private readonly List<Enrollment> _enrollments = new();
    private readonly List<StudentSkill> _skills = new();
    private readonly List<StudentExperience> _experiences = new();
    private readonly List<StudentProject> _projects = new();
    public Guid UserId { get; private set; }
    public string FullName { get; private set; }
    public string Faculty { get; private set; }
    public int Age { get; private set; }
    public string? Bio { get; private set; }
    public Gender Gender { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public string? ResumeUrl { get; private set; }
    public EgyptianUniversity University { get; private set; }
    public Year EnrollmentYear { get; private set; }
    public Year GraduationYear { get; private set; }
    public string Role { get; private set; }
    public string? Location { get; private set; }
    public IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyList<StudentSkill> Skills => _skills.AsReadOnly();
    public IReadOnlyList<StudentExperience> Experiences => _experiences.AsReadOnly();
    public IReadOnlyList<StudentProject> Projects => _projects.AsReadOnly();

    private StudentProfile(
        string fullName,
        EgyptianUniversity university,
        string faculty,
        Year graduationYear,
        Year enrollmentYear,
        int age,
        Gender gender,
        PhoneNumber phoneNumber,
        string? bio = null,
        string? profilePictureUrl = null)
    {
        FullName = fullName;
        University = university;
        Faculty = faculty;
        GraduationYear = graduationYear;
        EnrollmentYear = enrollmentYear;
        Age = age;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Bio = bio;
        ProfilePictureUrl = profilePictureUrl;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<StudentProfile> Create(
        string fullName,
        string university,
        string faculty,
        int graduationYear,
        int enrollmentYear,
        int age,
        string gender,
        string phoneNumber,
        string? bio = null,
        string? profilePictureUrl = null)
    {
        var genderResult = Enum.Parse<Gender>(gender);
        var universityResult = Enum.Parse<EgyptianUniversity>(university);
        var phoneResult = PhoneNumber.Create(phoneNumber);

        if (phoneResult.IsFailure)
            return Result.Failure<StudentProfile>(phoneResult.Error);

        var graduationYearResult = Year.Create(graduationYear);
        if (graduationYearResult.IsFailure)
            return Result.Failure<StudentProfile>(graduationYearResult.Error);

        var enrollmentYearResult = Year.Create(enrollmentYear);
        if (graduationYearResult.IsFailure)
            return Result.Failure<StudentProfile>(graduationYearResult.Error);

        return Result.Success(new StudentProfile(
            fullName.Trim(),
            universityResult,
            faculty,
            graduationYearResult.Value,
            enrollmentYearResult.Value,
            age,
            genderResult,
            phoneResult.Value,
            bio, profilePictureUrl));
    }

    public Result AddSkill(Skill skill)
    {
        if (Skills.Any(s => s.SkillId == skill.Id))
            return Result.Failure(DomainErrors.StudentErrors.DuplicateSkill);

        _skills.Add(new StudentSkill(Id, skill.Id));
        return Result.Success();
    }

    public void RemoveSkill(StudentSkill skill)
    {
        _skills.Remove(skill);
    }

    public Result UpdateInformation(
        string fullName,
        string? phoneNumber,
        string? location,
        int age,
        string gender
    )
    {
        var genderResult = Enum.Parse<Gender>(gender);

        FullName = fullName;
        if (phoneNumber != null)
        {
            var phoneResult = PhoneNumber.Create(phoneNumber);

            if (phoneResult.IsFailure)
                return Result.Failure<StudentProfile>(phoneResult.Error);
            PhoneNumber = phoneResult.Value;
        }

        Location = location;
        Age = age;
        Gender = genderResult;
        return Result.Success();
    }

    public Result UpdateEducation(string university, string faculty, int graduationYear, int enrollmentYear,
        string role)
    {
        var universityResult = Enum.Parse<EgyptianUniversity>(university);
        var graduationYearResult = Year.Create(graduationYear);
        var enrollmentYearResult = Year.Create(enrollmentYear);

        University = universityResult;
        Faculty = faculty;
        GraduationYear = graduationYearResult.Value;
        EnrollmentYear = enrollmentYearResult.Value;
        Role = role;
        return Result.Success(true);
    }

    public Result UpdateProfilePicture(string? profilePictureUrl)
    {
        if (profilePictureUrl is null)
            profilePictureUrl = null;
        ProfilePictureUrl = profilePictureUrl?.Trim();
        return Result.Success();
    }

    public Result UpdateResumeUrl(string? resumeUrl)
    {
        if (resumeUrl is null)
            resumeUrl = null;
        ResumeUrl = resumeUrl?.Trim();
        return Result.Success();
    }

    public Result UpdateBio(string bio)
    {
        Bio = bio;
        return Result.Success();
    }
}