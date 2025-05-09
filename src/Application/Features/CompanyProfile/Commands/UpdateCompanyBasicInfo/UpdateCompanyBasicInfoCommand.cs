using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyBasicInfo;

public sealed record UpdateCompanyBasicInfoCommand(
    Guid UserId,
    string Industry,
    string WebsiteUrl,
    string CompanySize,
    string YearOfEstablishment) : ICommand<bool>;