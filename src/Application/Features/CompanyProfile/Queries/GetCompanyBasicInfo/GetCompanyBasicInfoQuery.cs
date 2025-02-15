using Application.Abstractions.Messaging;
using Application.Features.CompanyProfile.Dtos;

namespace Application.Features.CompanyProfile.Queries.GetCompanyBasicInfo;

public record GetCompanyBasicInfoQuery(Guid UserId) : IQuery<CompanyBasicInfoDto>;