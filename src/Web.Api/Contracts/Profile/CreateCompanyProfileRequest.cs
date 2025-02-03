namespace Web.Api.Contracts.Profile;

public sealed record CreateCompanyProfileRequest(
    Guid UserId,
    string CompanyName,
    string TaxId,
    string Governorate,
    string Industry);
