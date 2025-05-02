using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.RemoveStudentProject;

public sealed record RemoveStudentProjectCommand( Guid StudentId, Guid ProjectId) : ICommand<bool>;