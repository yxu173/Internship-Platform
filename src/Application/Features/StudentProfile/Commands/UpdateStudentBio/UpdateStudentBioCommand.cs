using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.UpdateStudentBio;

public sealed record UpdateStudentBioCommand(
    Guid UserId,
    string Bio
  ) : ICommand<bool>;