namespace Web.Api.Contracts.Profile;

public sealed record CreateStudentProfileRequest(
    string FullName,
    string University,
    string Faculty,
    int GraduationYear,
    int EnrollmentYear,
    int Age,
    string Gender,
    string PhoneNumber,
    string? Bio,
    string? ProfilePictureUrl);