using FluentValidation;

namespace Application.Features.StudentProfile.Commands.UpdateStudentInfo;

public sealed class UpdateStudentInfoCommandValidator : AbstractValidator<UpdateStudentInfoCommand>
{
    public UpdateStudentInfoCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.");
        RuleFor(x => x.Age)
            .NotEmpty()
            .WithMessage("Age is required.");
        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithMessage("Gender is required.");
    }
}