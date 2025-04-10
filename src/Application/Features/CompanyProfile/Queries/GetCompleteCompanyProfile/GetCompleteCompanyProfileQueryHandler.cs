using Application.Abstractions.Messaging;
using Application.Features.CompanyProfile.Dtos;
using Application.Features.Internships.GetInternshipsByCompanyId;
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
        var company = await _companyRepository.GetByCompanyProfileWithInternships(request.Id,
            p => new CompleteCompanyProfile(
                p.LogoUrl,
                new CompanyBasicInfoDto(
                    p.CompanyName,
                    p.Description,
                    p.Industry,
                    p.Size.ToString(),
                    p.WebsiteUrl),
                new CompanyAddressDto(
                    p.Address.Governorate.ToString(),
                    p.Address.City,
                    p.Address.Street
                ),
                new CompanyAboutDto(
                    p.About.About,
                    p.About.Mission,
                    p.About.Vision),
                p.Internships.Select(i => new CompanyInternshipsDto(
                    i.Id,
                    i.Title,
                    i.Type.ToString(),
                    i.WorkingModel.ToString(),
                    i.Salary.Amount,
                    i.Salary.Currency,
                    i.IsActive
                )).ToList()
            ));

        if (company.IsFailure)
            return Result.Failure<CompleteCompanyProfile>(company.Error);


        return Result.Success(company.Value);
    }
}