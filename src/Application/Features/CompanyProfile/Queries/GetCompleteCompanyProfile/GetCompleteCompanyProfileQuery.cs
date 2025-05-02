using Application.Abstractions.Messaging;
using Application.Features.CompanyProfile.Dtos;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompleteCompanyProfile;

public sealed record GetCompleteCompanyProfileQuery(Guid Id) : IQuery<CompleteCompanyProfile>;