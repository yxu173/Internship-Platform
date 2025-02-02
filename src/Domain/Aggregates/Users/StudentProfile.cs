using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Users;

public sealed class StudentProfile : BaseAuditableEntity
{
    private StudentProfile() { }
    private readonly List<StudentSkill> _skills = new();
    public Guid UserId { get; private set; }
    public string FullName { get; private set; }
    public string Faculty { get; private set; }
    public int Age { get; private set; }
    public string? Bio { get; private set; }
    public Gender Gender { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public EgyptianUniversity University { get; private set; }
    public Year GraduationYear { get; private set; }
    public IReadOnlyList<StudentSkill> Skills => _skills.AsReadOnly();

    private StudentProfile(
        string fullName,
        EgyptianUniversity university,
        string faculty,
        Year graduationYear,
        int age,
        Gender gender,
        PhoneNumber phoneNumber)
    {
        FullName = fullName;
        University = university;
        Faculty = faculty;
        GraduationYear = graduationYear;
        Age = age;
        Gender = gender;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<StudentProfile> Create(
        string fullName,
        EgyptianUniversity university,
        string faculty,
        int graduationYear,
        int age,
        Gender gender,
        string phoneNumber)
    {
        var phoneResult = PhoneNumber.Create(phoneNumber);
        if (phoneResult.IsFailure)
            return Result.Failure<StudentProfile>(phoneResult.Error);

        var graduationYearResult = Year.Create(graduationYear);
        if (graduationYearResult.IsFailure)
            return Result.Failure<StudentProfile>(graduationYearResult.Error);

        return Result.Success(new StudentProfile(
            fullName.Trim(),
            university,
            faculty,
            graduationYearResult.Value,
            age,
            gender,
            phoneResult.Value));
    }

    public Result AddSkill(Skill skill)
    {
        if (Skills.Any(s => s.SkillId == skill.Id))
            return Result.Failure(DomainErrors.StudentErrors.DuplicateSkill);

        _skills.Add(new StudentSkill(Id, skill.Id));
        return Result.Success();
    }
}