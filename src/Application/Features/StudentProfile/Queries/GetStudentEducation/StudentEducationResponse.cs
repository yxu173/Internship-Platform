namespace Application.Features.StudentProfile.Queries.GetStudentEducation;

public sealed record StudentEducationResponse(
    string University,
    string Faculty,
    int GraduationYear,
    int EnrollmentYear,
    string Role);