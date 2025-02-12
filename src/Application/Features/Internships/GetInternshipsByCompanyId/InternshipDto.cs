namespace Application.Features.Internships.GetInternshipsByCompanyId;

public sealed record InternshipDto(
    Guid Id,
    string Title,
    string Description, 
    DateTime StartDate,
    DateTime EndDate,
    string Type,
    bool IsActive,
    DateTime CreatedAt);
