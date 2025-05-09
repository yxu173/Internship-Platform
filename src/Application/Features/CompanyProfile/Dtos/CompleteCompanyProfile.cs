using Application.Features.CompanyProfile.Queries.GetCompanyContact;
using Application.Features.CompanyProfile.Queries.GetCompanyInfo;
using Application.Features.Internships.GetInternshipsByCompanyId;

namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompleteCompanyProfile(
    string Logo,
    CompanyInfoResponse Info,
    CompanyBasicInfoDto BasicInfo,
    CompanyContactResponse Address,
    IReadOnlyList<CompanyInternshipsDto> Internships
);

//Roadmaps