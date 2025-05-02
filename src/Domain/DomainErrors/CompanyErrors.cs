using SharedKernel;

namespace Domain.DomainErrors;

public static class CompanyErrors
{
    public static Error TaxIdRequired => Error.BadRequest(
        "Company.TaxIdRequired",
        "The tax id is required");

    public static Error InvalidTaxIdFormat => Error.BadRequest(
        "Company.InvalidTaxIdFormat",
        "The tax id format is invalid");


    public static Error AlreadyRegistered => Error.Conflict(
        "Company.AlreadyRegistered",
        "The company is already registered");

    public static Error ProfileNotFound = Error.NotFound(
        "Company.ProfileNotFound",
        "Company profile not found");

    public static Error InvalidCompanySize => Error.BadRequest(
        "Company.InvalidCompanySize",
        "The company size is invalid");

    public static Error InvalidIndustry => Error.BadRequest(
        "Company.InvalidIndustry",
        "The industry is invalid");

    public static Error InvalidWebsiteUrl => Error.BadRequest(
        "Company.InvalidWebsiteUrl",
        "The website url is invalid");

    public static Error InvalidLogoUrl => Error.BadRequest(
        "Company.InvalidLogoUrl",
        "The logo url is invalid");

    public static Error InvalidDescription => Error.BadRequest(
        "Company.InvalidDescription",
        "The description is invalid");

    public static Error InvalidAddress => Error.BadRequest(
        "Company.InvalidAddress",
        "The address is invalid");

    public static Error InvalidGovernorate => Error.BadRequest(
        "Company.InvalidGovernorate",
        "The governorate is invalid");

    public static Error InvalidCity => Error.BadRequest(
        "Company.InvalidCity",
        "The city is invalid");

    public static Error DuplicateRoadmap => Error.Conflict(
        "Company.DuplicateRoadmap",
        "The roadmap is already added to the company profile"
    );
}