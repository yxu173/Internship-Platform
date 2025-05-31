using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.UpdateStudentEducation;

public sealed record UpdateStudentEducationCommand(
    Guid UserId,
    string University,
    string Faculty,
    int GraduationYear,
    int EnrollmentYear,
    string Role
  ) : ICommand<bool>;