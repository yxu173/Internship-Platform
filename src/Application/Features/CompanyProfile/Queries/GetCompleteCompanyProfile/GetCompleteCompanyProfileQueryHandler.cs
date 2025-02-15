using Application.Abstractions.Messaging;
using Application.Features.CompanyProfile.Dtos;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompleteCompanyProfile;

public sealed class
    GetCompleteCompanyProfileQueryHandler : IQueryHandler<GetCompleteCompanyProfileQuery, CompleteCompanyProfile>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompleteCompanyProfileQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompleteCompanyProfile>> Handle(GetCompleteCompanyProfileQuery request,
        CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByUserIdAsync(request.Id,
            p => new CompleteCompanyProfile(
                p.LogoUrl,
                new CompanyBasicInfoDto(
                    p.CompanyName,
                    p.Description,
                    p.Industry,
                    p.Size.ToString(),
                    p.WebsiteUrl),
                new CompanyAboutDto(
                    p.About.About,
                    p.About.Mission,
                    p.About.Vision)
            ));

        if (company.IsFailure)
            return Result.Failure<CompleteCompanyProfile>(company.Error);

        return Result.Success(company.Value);
    }
}