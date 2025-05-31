namespace Web.Api.Contracts.Profile;

public sealed record UpdateEducationRequest(
    string University,
    string Faculty,
    int GraduationYear,
    int EnrollmentYear,
    string Role
);