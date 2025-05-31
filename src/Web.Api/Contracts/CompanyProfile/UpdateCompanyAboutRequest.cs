namespace Web.Api.Contracts.CompanyProfile;

public sealed record UpdateCompanyAboutRequest(
    string About,
    string Mission,
    string Vision
);