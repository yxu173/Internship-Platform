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

    public static Error PublicUniversityGraduationLimit => Error.Validation(
        "Student.PublicUniversityGraduationLimit",
        "Public university graduates must apply within 2 years of graduation");

    public static Error SinaiPrivateUniversityRestriction => Error.Validation(
        "Student.SinaiPrivateUniversityRestriction",
        "Private university students from North Sinai require special approval");

    public static Error DuplicateSkill => Error.Conflict(
        "Student.DuplicateSkill",
        "Skill already exists in profile");

    public static Error InvalidGraduationYear => Error.Validation(
        "Student.InvalidGraduationYear",
        $"Graduation year must be between 2000-{DateTime.Now.Year + 5}");

    public static Error AlreadyExists => Error.Conflict(
        "Student.AlreadyExists",
        "Student profile already exists");
}