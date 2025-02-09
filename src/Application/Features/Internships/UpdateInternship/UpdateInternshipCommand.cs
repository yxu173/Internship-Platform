using Application.Abstractions.Messaging;

namespace Application.Features.Internships.UpdateInternship;
public sealed record UpdateInternshipCommand(
    Guid InternshipId,
    string Title,
    string Description,
    DateTime ApplicationDeadline
) : ICommand<bool>;