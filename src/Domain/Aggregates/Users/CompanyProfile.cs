using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Users;

public class CompanyProfile : BaseAuditableEntity
{
    public string CompanyName { get; }
    public EgyptianTaxId TaxId { get; }
    public string Industry { get; private set; }
    public string WebsiteUrl { get; private set; }
    public string Description { get; private set; }
    public CompanySize Size { get; private set; }
    public Governorate Governorate { get; private set; }
    public DateTime RegistrationDate { get; }

    private CompanyProfile(
        string companyName,
        EgyptianTaxId taxId,
        Governorate governorate)
    {
        CompanyName = companyName;
        TaxId = taxId;
        Governorate = governorate;
        RegistrationDate = DateTime.UtcNow;
    }

    public static Result<CompanyProfile> Create(
        string companyName,
        string taxId,
        Governorate governorate)
    {
        var taxIdResult = EgyptianTaxId.Create(taxId);
        if (taxIdResult.IsFailure)
            return Result.Failure<CompanyProfile>(taxIdResult.Error);


        return new CompanyProfile(
            companyName.Trim(),
            taxIdResult.Value,
            governorate);
    }

    public Result UpdateDetails(
        string industry,
        string websiteUrl,
        string description,
        CompanySize size)
    {
        Industry = industry;
        WebsiteUrl = websiteUrl;
        Description = description;
        Size = size;

        return Result.Success();
    }
}