using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Users;

public sealed class StudentProfile : BaseAuditableEntity
{
    public string FullName { get; }
    public string? Faculty { get; }
    public int Age { get; }
    public string? Bio { get; }
    public Gender Gender { get; }
    public string? PhoneNumber { get; }
    public string? ProfilePictureUrl { get; }

    public EgyptianUniversity University { get; }
    public int GraduationYear { get; }
    public List<StudentSkill> Skills { get; } = new();


    public StudentProfile(
        string fullName,
        EgyptianUniversity university,
        string faculty,
        int graduationYear,
        int age,
        string bio,
        Gender gender,
        string phoneNumber,
        string profilePictureUrl)
    {
        FullName = fullName;
        University = university;
        Faculty = faculty;
        GraduationYear = graduationYear;
        Age = age;
        Bio = bio;
        Gender = gender;
        PhoneNumber = phoneNumber;
        ProfilePictureUrl = profilePictureUrl;
    }

    public static Result<StudentProfile> Create(
        string fullName,
        EgyptianUniversity university,
        string faculty,
        int graduationYear,
        int age,
        string bio,
        Gender gender,
        string phoneNumber,
        string profilePictureUrl)
    {
        var phoneResult = ValueObjects.PhoneNumber.Create(phoneNumber);
        if (phoneResult.IsFailure)
            return Result.Failure<StudentProfile>(phoneResult.Error);

        var graduationYearResult = Year.Create(graduationYear);
        if (graduationYearResult.IsFailure)
            return Result.Failure<StudentProfile>(graduationYearResult.Error);


        return new StudentProfile(
            fullName.Trim(), university,
            faculty,
            graduationYear,
            age,
            bio,
            gender,
            phoneNumber,
            profilePictureUrl);
    }

    public Result AddSkill(Skill skill)
    {
        if (Skills.Any(s => s.SkillId == skill.Id))
            return Result.Failure(DomainErrors.StudentErrors.DuplicateSkill);

        Skills.Add(new StudentSkill(Id, skill.Id));
        return Result.Success();
    }
}