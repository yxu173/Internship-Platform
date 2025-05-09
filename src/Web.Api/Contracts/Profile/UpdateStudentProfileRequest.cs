namespace Web.Api.Contracts.Profile;

public record UpdateStudentProfileRequest(
    string FullName,
    string? PhoneNumber,
    string? Location,
    int Age,
    string Gender
);