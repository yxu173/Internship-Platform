using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;

namespace Application.Features.Skills.GetAllSkills
{
    public sealed record GetAllSkillsQuery : IQuery<IEnumerable<Skill>>;
}