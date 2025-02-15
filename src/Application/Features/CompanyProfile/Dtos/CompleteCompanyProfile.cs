namespace Application.Features.CompanyProfile.Dtos;

public sealed record CompleteCompanyProfile(
    string Logo,
    CompanyBasicInfoDto BasicInfo,
    CompanyAboutDto About
);

//Internships - Roadmaps