namespace Web.Api.Contracts.CompanyProfile;

public sealed record UpdateCompanyBasicInfoRequest(
    string Industry,
    string WebsiteUrl,
    string CompanySize,
    string yearOfEstablishment
);