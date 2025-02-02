using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Users;

public sealed class CompanyProfile : BaseAuditableEntity
{
    private CompanyProfile() { }
    public Guid UserId { get; private set; }
    public string CompanyName { get; private set; }
    public EgyptianTaxId TaxId { get; private set; }
    public string Industry { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? Description { get; private set; }
    public CompanySize? Size { get; private set; }
    public Governorate Governorate { get; private set; }


    private CompanyProfile(
        string companyName,
        EgyptianTaxId taxId,
        Governorate governorate,
        string industry
    )
    {
        CompanyName = companyName;
        TaxId = taxId;
        Governorate = governorate;
        Industry = industry;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<CompanyProfile> Create(
        string companyName,
        string taxId,
        Governorate governorate,
        string industry)
    {
        var taxIdResult = EgyptianTaxId.Create(taxId);
        if (taxIdResult.IsFailure)
            return Result.Failure<CompanyProfile>(taxIdResult.Error);

        return Result.Success(new CompanyProfile(
            companyName.Trim(),
            taxIdResult.Value,
            governorate,
            industry.Trim()
        ));
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