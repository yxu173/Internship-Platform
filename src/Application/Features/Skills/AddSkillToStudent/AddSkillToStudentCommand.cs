using Application.Abstractions.Messaging;

namespace Application.Features.Skills;

public sealed record AddSkillToStudentCommand(Guid UserId, Guid SkillId) 
    : ICommand<bool>;
