namespace Web.Api.Contracts.Profile;
public sealed record CreateStudentProfileRequest(
    Guid UserId,
    string FullName,
    string University,
    string Faculty,
    int GraduationYear,
    int Age,
    string Gender,
    string PhoneNumber);