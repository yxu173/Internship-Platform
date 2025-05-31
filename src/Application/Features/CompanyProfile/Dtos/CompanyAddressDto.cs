namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompanyAddressDto(
    string Email,
    string Governorate,
    string City);