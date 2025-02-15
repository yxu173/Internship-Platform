using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyAbout;

public sealed record UpdateCompanyAboutCommand(
    Guid UserId,
    string About,
    string Mission,
    string Vision
) : ICommand<bool>;