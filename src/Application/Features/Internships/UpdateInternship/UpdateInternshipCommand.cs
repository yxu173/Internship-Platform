using Application.Abstractions.Messaging;

namespace Application.Features.Internships.UpdateInternship;
public sealed record UpdateInternshipCommand(
    Guid InternshipId,
    string Title,
    string About,
    string KeyResponsibilities,
    string Requirements,
    string Type,
    string WorkingModel,
    decimal Salary,
    string Currency,
    DateTime ApplicationDeadline
) : ICommand<bool>;