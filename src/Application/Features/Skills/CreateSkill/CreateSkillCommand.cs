using Application.Abstractions.Messaging;

namespace Application.Features.Skills.CreateSkill
{
    public sealed record CreateSkillCommand(string Name) : ICommand<bool>;
}