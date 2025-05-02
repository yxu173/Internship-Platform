namespace Application.Features.StudentProfile.Queries.GetAllStudentProfile;

public sealed record CompleteStudentProfileResponse(
    Guid Id,
    BasicInfoResponse BasicInfo,
    List<SkillResponse> Skills,
    List<ExperienceResponse> Experiences,
    List<ProjectResponse> Projects
);

public sealed record BasicInfoResponse(
    string FullName,
    string University,
    string Faculty,
    int GraduationYear,
    int EnrollmentYear,
    int Age,
    string Gender,
    string Bio,
    string PhoneNumber,
    string ProfilePictureUrl,
    string ResumeUrl
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