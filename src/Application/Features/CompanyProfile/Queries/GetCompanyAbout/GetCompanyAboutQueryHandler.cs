using Application.Abstractions.Messaging;
using Application.Features.CompanyProfile.Dtos;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompanyAbout;

public class GetCompanyAboutQueryHandler : IQueryHandler<GetCompanyAboutQuery, CompanyAboutDto>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyAboutQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompanyAboutDto>> Handle(GetCompanyAboutQuery request,
        CancellationToken cancellationToken)
    {
        var about = await _companyRepository.GetByUserIdAsync(request.UserId,
            p => new CompanyAboutDto(
                p.About.About,
                p.About.Mission,
                p.About.Vision
            )
        );
        return Result.Success<CompanyAboutDto>(about.Value);
    }
}