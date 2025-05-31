using Application.Abstractions.Messaging;
using Application.Features.CompanyProfile.Dtos;

namespace Application.Features.CompanyProfile.Queries.GetCompanyAbout;

public sealed record GetCompanyAboutQuery(Guid UserId) : IQuery<CompanyAboutDto>;