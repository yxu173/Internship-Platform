namespace Web.Api.Contracts.CompanyProfile;

public sealed record UpdateCompanyBasicInfoRequest(
    string Name,
    string Industry,
    string Description,
    string WebsiteUrl,
    string CompanySize
);