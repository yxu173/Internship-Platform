namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompanyInternshipsDto(
    Guid Id,
    string Title,
    string Type,
    int ApplicationCount,
    string Currency,
    bool IsActive
    );