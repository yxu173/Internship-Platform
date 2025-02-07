namespace Web.Api.Contracts.Profile;

public sealed record CreateCompanyProfileRequest(
    string CompanyName,
    string TaxId,
    string Governorate,
    string Industry);
