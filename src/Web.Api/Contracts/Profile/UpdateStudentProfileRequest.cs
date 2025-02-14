namespace Web.Api.Contracts.Profile;

public record UpdateStudentProfileRequest(
    string FullName,
    string University,
    string Faculty,
    int GraduationYear,
    int EnrollmentYear,
    int Age,
    string Bio,
    string Gender
    );