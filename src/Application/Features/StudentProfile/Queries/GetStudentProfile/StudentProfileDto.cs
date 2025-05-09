namespace Application.Features.StudentProfile.Queries.GetStudentProfile;

public sealed record StudentProfileDto(
    Guid StudentId,
    Guid UserId,
    string FullName,
    string? PhoneNumber,
    string? Location,
    int Age,
    string Gender
);