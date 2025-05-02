using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Queries.GetCompanyLogo;

public sealed record GetCompanyLogoQuery( Guid UserId) : IQuery<string?>;