using Application.Abstractions.Messaging;

namespace Application.Features.Profiles.GetCompanyProfile;

public sealed record GetCompanyProfileQuery(Guid UserId) 
    : IQuery<CompanyProfileDto>;
