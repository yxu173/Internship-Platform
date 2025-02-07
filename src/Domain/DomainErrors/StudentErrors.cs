using SharedKernel;

namespace Domain.DomainErrors;

public static class StudentErrors
{
    public static Error InvalidAge => Error.Validation(
        "Student.InvalidAge",
        "Age must be between 18-35 years for Egyptian internships");

    public static Error GraduationTooOld => Error.Validation(
        "Student.GraduationTooOld",
        "Graduation year cannot be older than 5 years");
    public static Error DuplicateSkill => Error.Conflict(
        "Student.DuplicateSkill",
        "Skill already exists in profile");

    public static Error ProfileNotFound => Error.NotFound(
        "Student.NotFound",
        "Student Profile Not Found"
    );
    public static Error InvalidGraduationYear => Error.Validation(
        "Student.InvalidGraduationYear",
        $"Graduation year must be between 2000-{DateTime.Now.Year + 5}");

    public static Error AlreadyExists => Error.Conflict(
        "Student.AlreadyExists",
        "Student profile already exists");

    public static Error SkillNotFound => Error.NotFound(
        "Student.SkillNotFound",
        "Skill not found in student profile");
}