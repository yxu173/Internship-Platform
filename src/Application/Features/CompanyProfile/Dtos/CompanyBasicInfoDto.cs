namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompanyBasicInfoDto(
    string CompanyName,
    string Description,
    string Industry,
    string Size,
    string WebsiteUrl
);