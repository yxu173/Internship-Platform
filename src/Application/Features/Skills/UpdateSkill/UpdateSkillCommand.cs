using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;

namespace Application.Features.Skills.UpdateSkill
{
    public sealed record UpdateSkillCommand(Guid Id) : ICommand<Skill>;
}