using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Commands.CreateCompanyProfile;

public sealed record CreateCompanyProfileCommand(
    Guid UserId,
    string CompanyName,
    string TaxId,
    string Governorate,
    string City,
    string Industry) : ICommand<Guid>;