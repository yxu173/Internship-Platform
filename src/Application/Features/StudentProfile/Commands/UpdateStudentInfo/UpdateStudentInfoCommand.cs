using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.UpdateStudentInfo;

public sealed record UpdateStudentInfoCommand(
    Guid UserId,
    string FullName,
    string University,
    string Faculty,
    int EnrollmentYear,
    int GraduationYear,
    int Age,
    string Bio,
    string Gender) : ICommand<bool>;