using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.CreateStudentProject;

public sealed record CreateStudentProjectCommand(
    Guid UserId,
    string ProjectName,
    string Description,
    string ProjectUrl) : ICommand<bool>;