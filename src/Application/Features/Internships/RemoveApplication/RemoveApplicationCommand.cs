using Application.Abstractions.Messaging;

namespace Application.Features.Internships.RemoveApplication;

public sealed record RemoveApplicationCommand(
    Guid ApplicationId,
    Guid UserId
) : ICommand<bool>;