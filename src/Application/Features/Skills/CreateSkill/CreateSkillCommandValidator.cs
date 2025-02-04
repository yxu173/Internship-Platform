using FluentValidation;

namespace Application.Features.Skills.CreateSkill;

public sealed class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
{
    public CreateSkillCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Skill name is required.");
    }
}