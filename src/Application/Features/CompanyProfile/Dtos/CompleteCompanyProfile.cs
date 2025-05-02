using Application.Features.Internships.GetInternshipsByCompanyId;

namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompleteCompanyProfile(
    string Logo,
    CompanyBasicInfoDto BasicInfo,
    CompanyAddressDto Address,
    CompanyAboutDto About,
    IReadOnlyList<CompanyInternshipsDto> Internships
);

//Roadmaps