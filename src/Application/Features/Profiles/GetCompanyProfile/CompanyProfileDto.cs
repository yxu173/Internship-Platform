namespace Application.Features.Profiles.GetCompanyProfile;

public sealed record CompanyProfileDto(
    Guid CompanyId,
    Guid UserId,
    string CompanyName,
    string Governorate,
    string Industry,
    string? WebsiteUrl,
    string? Description,
    string CompanySize
);