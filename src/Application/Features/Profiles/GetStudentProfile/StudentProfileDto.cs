namespace Application.Features.Profiles.GetStudentProfile;

public sealed record StudentProfileDto(
    string FullName,
    string University,
    string Faculty,
    int GraduationYear,
    int Age,
    string? Bio,
    List<string> Skills
);