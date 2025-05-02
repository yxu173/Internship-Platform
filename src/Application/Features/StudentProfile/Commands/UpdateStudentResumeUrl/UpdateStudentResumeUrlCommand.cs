using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.UpdateStudentResumeUrl;

public sealed record UpdateStudentResumeUrlCommand(Guid UserId, string ResumeUrl) : ICommand<bool>;