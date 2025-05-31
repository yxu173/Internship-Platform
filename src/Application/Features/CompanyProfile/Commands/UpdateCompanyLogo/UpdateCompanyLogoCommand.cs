using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyLogo;

public sealed record UpdateCompanyLogoCommand(
    Guid UserId,
    string LogoUrl
) : ICommand<bool>;