using FluentValidation;

namespace Application.Features.StudentProfile.Commands.UpdateStudentInfo;

public sealed class UpdateStudentInfoCommandValidator : AbstractValidator<UpdateStudentInfoCommand>
{
    public UpdateStudentInfoCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.");
        //TODO: Add more validation rules for university
        RuleFor(x => x.University)
            .NotEmpty()
            .WithMessage("University is required.");
        RuleFor(x => x.Faculty)
            .NotEmpty()
            .WithMessage("Faculty is required.");
        RuleFor(x => x.EnrollmentYear)
            .NotEmpty()
            .WithMessage("Enrollment year is required.");
        RuleFor(x => x.GraduationYear)
            .NotEmpty()
            .WithMessage("Graduation year is required.");
        RuleFor(x => x.Age)
            .NotEmpty()
            .WithMessage("Age is required.");
        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithMessage("Gender is required.");
    }
}