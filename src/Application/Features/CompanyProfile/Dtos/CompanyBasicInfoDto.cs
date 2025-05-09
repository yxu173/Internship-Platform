namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompanyBasicInfoDto(
    string Industry,
    string Size,
    string WebsiteUrl,
    string YearOfEstablishment
);