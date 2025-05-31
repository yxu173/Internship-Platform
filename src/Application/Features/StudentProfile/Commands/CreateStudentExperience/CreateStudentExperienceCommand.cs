using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.CreateStudentExperience;

public sealed record CreateStudentExperienceCommand(
    Guid UserId,
    string JobTitle,
    string CompanyName,
    DateTime StartDate,
    DateTime EndDate
) : ICommand<Guid>;