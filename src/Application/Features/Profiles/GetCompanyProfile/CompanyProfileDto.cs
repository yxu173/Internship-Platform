namespace Application.Features.Profiles.GetCompanyProfile;

public sealed record CompanyProfileDto(
    Guid CompanyId,
    string CompanyName,
    string Governorate,
    string Industry,
    string? WebsiteUrl,
    string? Description,
    string CompanySize
);