using Application.Abstractions.Messaging;
using Application.Features.CompanyProfile.Dtos;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompanyBasicInfo;

public class GetCompanyBasicInfoQueryHandler : IQueryHandler<GetCompanyBasicInfoQuery, CompanyBasicInfoDto>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyBasicInfoQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompanyBasicInfoDto>> Handle(GetCompanyBasicInfoQuery request,
        CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByUserIdAsync(request.UserId,
            p => new CompanyBasicInfoDto(
                p.CompanyName,
                p.Description,
                p.Industry,
                p.Size.ToString(),
                p.WebsiteUrl
            )
        );

        return Result.Success<CompanyBasicInfoDto>(company.Value);
    }
}