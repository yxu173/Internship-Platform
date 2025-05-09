using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Queries.GetCompanyContact;

public sealed record GetCompanyContactQuery(Guid UserId) : IQuery<CompanyContactResponse>;