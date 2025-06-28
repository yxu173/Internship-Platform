using Application.Abstractions.Messaging;

namespace Application.Features.Internships.CreateApplication;

public sealed record CreateApplicationCommand(
    Guid InternshipId,
    Guid UserId,
    string ResumeUrl
) : ICommand<Guid>;