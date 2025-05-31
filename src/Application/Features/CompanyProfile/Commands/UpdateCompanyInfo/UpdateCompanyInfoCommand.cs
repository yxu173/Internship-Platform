using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyInfo;

public sealed record UpdateCompanyInfoCommand(Guid UserId, string Name, string Description) : ICommand<bool>;