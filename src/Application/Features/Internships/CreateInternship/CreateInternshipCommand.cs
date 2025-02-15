using Application.Abstractions.Messaging;
using Domain.ValueObjects;

namespace Application.Features.Internships.CreateInternship;

public sealed record CreateInternshipCommand(
    Guid UserId,
    string Title,
    string About,
    string KeyResponsibilities,
    string Requirements,
    string Type,
    string WorkingModel,
    decimal Salary,
    string Currency,
    DateTime StartDate,
    DateTime EndDate,
    DateTime ApplicationDeadline
) : ICommand<Guid>;