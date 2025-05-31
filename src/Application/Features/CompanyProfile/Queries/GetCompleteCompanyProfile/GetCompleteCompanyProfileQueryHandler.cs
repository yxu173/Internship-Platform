using Application.Abstractions.Messaging;
using Application.Features.CompanyProfile.Dtos;
using Application.Features.CompanyProfile.Queries.GetCompanyContact;
using Application.Features.CompanyProfile.Queries.GetCompanyInfo;
using Application.Features.Internships.GetInternshipsByCompanyId;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompleteCompanyProfile;

public sealed class
    GetCompleteCompanyProfileQueryHandler : IQueryHandler<GetCompleteCompanyProfileQuery, CompleteCompanyProfile>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;

    public GetCompleteCompanyProfileQueryHandler(ICompanyRepository companyRepository, IUserRepository userRepository)
    {
        _companyRepository = companyRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<CompleteCompanyProfile>> Handle(GetCompleteCompanyProfileQuery request,
        CancellationToken cancellationToken)
    {
        var companyProfile = await _companyRepository.GetByCompanyIdAsync(request.Id);
        var user = await _userRepository.GetByIdAsync(companyProfile.UserId);
        var company = await _companyRepository.GetByCompanyProfileWithInternships(companyProfile.UserId,
            p => new CompleteCompanyProfile(
                string.IsNullOrEmpty(p.LogoUrl) ? "/uploads/company-logos/default-logo.png" : p.LogoUrl,
                new CompanyInfoResponse(
                    p.CompanyName,
                    p.Description),
                new CompanyBasicInfoDto(
                    p.Industry,
                    p.Size.ToString(),
                    p.WebsiteUrl,
                    p.YearOfEstablishment),
                new CompanyContactResponse(
                    user.Email,
                    p.Address.Governorate.ToString(),
                    p.Address.City
                ),
                p.Internships.Select(i => new CompanyInternshipsDto(
                    i.Id,
                    i.Title,
                    i.Type.ToString(),
                    i.Applications.Count,
                    i.Salary.Currency,
                    i.IsActive
                )).ToList()
            ));

        if (company.IsFailure)
            return Result.Failure<CompleteCompanyProfile>(company.Error);


        return Result.Success(company.Value);
    }
}