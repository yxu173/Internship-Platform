namespace Application.Features.StudentProfile.Queries.GetAllStudentProfile;

public sealed record CompleteStudentProfileResponse(
    Guid Id,
    string Bio,
    string ProfilePictureUrl,
    string ResumeUrl,
    BasicInfoResponse BasicInfo,
    EducationResponse Education,
    List<SkillResponse> Skills,
    List<ExperienceResponse> Experiences,
    List<ProjectResponse> Projects
);

public sealed record BasicInfoResponse(
    string FullName,
    int Age,
    string Gender,
    string PhoneNumber,
    string Location,
    string Email
);

public sealed record EducationResponse(
    string University,
    string Faculty,
    int GraduationYear,
    int EnrollmentYear,
    string Role
);

public sealed record SkillResponse(
    Guid Id,
    string Name
);

public sealed record ExperienceResponse(
    Guid Id,
    string JobTitle,
    string CompanyName,
    DateTime StartDate,
    DateTime? EndDate
);

public sealed record ProjectResponse(
    Guid Id,
    string ProjectName,
    string Description,
    string ProjectUrl
);

//public sealed record ApplicationResponse();