using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.RemoveStudentExperience;

public sealed record RemoveStudentExperienceCommand(Guid ExperienceId) : ICommand<bool>;