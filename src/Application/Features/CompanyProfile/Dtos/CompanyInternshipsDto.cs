namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompanyInternshipsDto(
    Guid Id,
    string Title,
    string Type,
    string WorkingModel,
    decimal Salary,
    string Currency,
    bool IsActive
    );