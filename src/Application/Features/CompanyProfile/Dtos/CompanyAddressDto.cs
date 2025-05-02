namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompanyAddressDto(
    string Governorate,
    string City,
    string Street
    );