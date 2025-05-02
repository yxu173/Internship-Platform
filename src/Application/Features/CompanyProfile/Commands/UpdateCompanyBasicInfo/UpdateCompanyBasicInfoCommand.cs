using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyBasicInfo;

public sealed record UpdateCompanyBasicInfoCommand(
    Guid UserId,
    string Name,
    string Industry,
    string Description,
    string WebsiteUrl,
    string CompanySize) : ICommand<bool>;