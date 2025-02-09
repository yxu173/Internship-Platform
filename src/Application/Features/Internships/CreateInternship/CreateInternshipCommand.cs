using Application.Abstractions.Messaging;
using Domain.ValueObjects;

namespace Application.Features.Internships.CreateInternship;
public sealed record CreateInternshipCommand(
    Guid UserId,
    string Title,
    string Description,
    string Type,
    DateTime StartDate,
    DateTime EndDate,
    DateTime ApplicationDeadline
) : ICommand<Guid>;