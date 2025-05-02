namespace Application.Features.Internships.GetInternshipsByCompanyId;

public sealed record InternshipDto(
    Guid Id,
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
