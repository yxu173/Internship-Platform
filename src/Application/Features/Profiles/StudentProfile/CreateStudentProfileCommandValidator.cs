using FluentValidation;

namespace Application.Features.Profiles.StudentProfile;

public sealed class CreateStudentProfileCommandValidator
        : AbstractValidator<CreateStudentProfileCommand>
{
    public CreateStudentProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

        RuleFor(x => x.University)
            .IsInEnum().WithMessage("Invalid university selection");

        RuleFor(x => x.Faculty)
            .NotEmpty().WithMessage("Faculty is required")
            .MaximumLength(100).WithMessage("Faculty cannot exceed 100 characters");

        RuleFor(x => x.GraduationYear.Value)
            .InclusiveBetween(2000, DateTime.Now.Year + 5)
            .WithMessage("Invalid graduation year");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 35).WithMessage("Age must be between 18-35");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("phone number is required");
    }
}