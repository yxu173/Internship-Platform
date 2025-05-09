using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Queries.GetCompanyInfo;

public sealed record GetCompanyInfoQuery(Guid UserId) : IQuery<CompanyInfoResponse>;