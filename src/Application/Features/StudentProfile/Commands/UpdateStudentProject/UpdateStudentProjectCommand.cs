using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.UpdateStudentProject;

public sealed record UpdateStudentProjectCommand(
    Guid StudentId,
    Guid ProjectId,
    string ProjectName,
    string Description,
    string ProjectUrl) : ICommand<bool>;