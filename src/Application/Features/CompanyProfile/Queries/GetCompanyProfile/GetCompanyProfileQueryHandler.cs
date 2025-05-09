using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompanyProfile;

public sealed class GetCompanyProfileQueryHandler 
    : IQueryHandler<GetCompanyProfileQuery, CompanyProfileDto>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyProfileQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompanyProfileDto>> Handle(
        GetCompanyProfileQuery request, 
        CancellationToken cancellationToken)
    {
        var profile = await _companyRepository.GetCompanyByIdAsync(request.UserId);
        
        if (profile is null)
            return Result.Failure<CompanyProfileDto>
            (Domain.DomainErrors.CompanyErrors.ProfileNotFound);

        return new CompanyProfileDto(
            profile.Id,
            profile.UserId,
            profile.CompanyName,
            profile.Address.Governorate.ToString(),
            profile.Address.City,
            profile.Industry,
            profile.WebsiteUrl,
            profile.Description,
            profile.Size?.ToString() ?? "NotSpecified");
    }
}
