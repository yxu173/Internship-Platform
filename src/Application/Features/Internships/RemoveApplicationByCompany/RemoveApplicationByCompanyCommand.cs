using Application.Abstractions.Messaging;

namespace Application.Features.Internships.RemoveApplicationByCompany;

public sealed record RemoveApplicationByCompanyCommand(
    Guid ApplicationId,
    Guid CompanyUserId
) : ICommand<bool>; 