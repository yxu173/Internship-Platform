using Application.Abstractions.Messaging;

namespace Application.Features.Skills;

public sealed record RemoveSkillFromStudentCommand(Guid UserId, Guid SkillId) 
    : ICommand<bool>;
