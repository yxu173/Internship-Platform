using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.CreateStudentProfile;

public sealed record CreateStudentProfileCommand(
    Guid UserId,
    string FullName,
    string University,
    string Faculty,
    int GraduationYear,
    int EnrollmentYear,
    int Age,
    string Gender,
    string PhoneNumber,
    string? Bio,
    string? ProfilePictureUrl
) : ICommand<Guid>;