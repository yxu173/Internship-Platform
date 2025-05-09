using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.UpdateStudentExperience;

public sealed record UpdateStudentExperienceCommand(
    Guid UserId,
    Guid ExperienceId,
    string JobTitle,
    string CompanyName,
    DateTime StartDate,
    DateTime EndDate
) : ICommand<bool>;