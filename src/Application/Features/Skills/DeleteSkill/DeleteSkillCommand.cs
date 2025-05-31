using Application.Abstractions.Messaging;

namespace Application.Features.Skills.DeleteSkill;
public sealed record DeleteSkillCommand(Guid SkillId) : ICommand<bool>;
