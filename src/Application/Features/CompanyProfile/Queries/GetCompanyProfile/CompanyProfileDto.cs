namespace Application.Features.CompanyProfile.Queries.GetCompanyProfile;

public sealed record CompanyProfileDto(
    Guid CompanyId,
    Guid UserId,
    string CompanyName,
    string Governorate,
    string City,
    string Street,
    string Industry,
    string? WebsiteUrl,
    string? Description,
    string CompanySize
);