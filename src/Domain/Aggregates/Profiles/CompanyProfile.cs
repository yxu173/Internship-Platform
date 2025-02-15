using Domain.Aggregates.Internships;
using Domain.Common;
using Domain.DomainErrors;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Profiles;

public sealed class CompanyProfile : BaseAuditableEntity
{
    //TODO: Company Information (CompanyName, TaxId, Industry, WebsiteUrl, LogoUrl, Description, Size, Address)
    //TODO: Company Contact Information (Email, PhoneNumber, WebsiteUrl)
    //TODO: Company Social Media (Facebook, Twitter, LinkedIn)
    //TODO: Company Internship (Internships, Applications)
    //TODO: Company About (About, Mission, Vision)

    private CompanyProfile()
    {
    }

    private readonly List<Internship> _internships = new();
    private readonly List<Application> _applications = new();
    public Guid UserId { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string CompanyName { get; private set; }
    public string? Description { get; private set; }
    public string Industry { get; private set; }
    public EgyptianTaxId TaxId { get; private set; }
    public CompanyAbout About { get; private set; }

    public Address Address { get; private set; }
    public CompanySize? Size { get; private set; }
    public IReadOnlyList<Internship> Internships => _internships.AsReadOnly();
    public IReadOnlyList<Application> Applications => _applications.AsReadOnly();


    private CompanyProfile(
        string companyName,
        EgyptianTaxId taxId,
        Address address,
        string industry
    )
    {
        CompanyName = companyName;
        TaxId = taxId;
        Address = address;
        Industry = industry;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<CompanyProfile> Create(
        string companyName,
        string taxId,
        string governorate,
        string city,
        string street,
        string industry)
    {
        var taxIdResult = EgyptianTaxId.Create(taxId);

        if (taxIdResult.IsFailure)
            return Result.Failure<CompanyProfile>(taxIdResult.Error);

        var addressResult = ValueObjects.Address.Create(governorate, city, street);
        if (addressResult.IsFailure)
            return Result.Failure<CompanyProfile>(addressResult.Error);

        return Result.Success(new CompanyProfile(
            companyName.Trim(),
            taxIdResult.Value,
            addressResult.Value,
            industry.Trim()
        ));
    }

    public Result UpdateDetails(
        string companyName,
        string industry,
        string websiteUrl,
        string description,
        string size)
    {
        CompanyName = companyName;
        Industry = industry;
        WebsiteUrl = websiteUrl;
        Description = description;
        Size = Enum.Parse<CompanySize>(size);

        return Result.Success();
    }
    public Result UpdateLogo(string logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
            return Result.Failure(CompanyErrors.InvalidLogoUrl);

        LogoUrl = logoUrl.Trim();
        return Result.Success();
    }
}