namespace Application.Features.Internships.GetByInternshipId;

public sealed record InternshipResponse(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    string CompanyLogoUrl,
    string Government,
    string City,
    string Title,
    string About,
    string KeyResponsibilities,
    string Requirements,
    DateTime StartDate,
    DateTime EndDate,
    string Type,
    string WorkingModel,
    decimal Salary,
    string Currency,
    bool IsActive,
    DateTime CreatedAt);