using Application.Abstractions.Messaging;

namespace Application.Features.CompanyProfile.Queries.GetCompanyProfile;

public sealed record GetCompanyProfileQuery(Guid UserId) 
    : IQuery<CompanyProfileDto>;
