namespace Application.Features.StudentProfile.Queries.GetAllStudentExperiences;

public sealed record StudentExperienceDto(
    Guid Id,
    string JobTitle,
    string CompanyName,
    DateTime StartDate,
    DateTime EndDate);