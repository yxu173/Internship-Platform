namespace Application.Features.Profiles.GetStudentProfile;

public sealed record StudentProfileDto(
    Guid StudentId,
    Guid UserId,
    string FullName,
    string University,
    string Faculty,
    int GraduationYear,
    int Age,
    string? Bio,
    List<string> Skills
);