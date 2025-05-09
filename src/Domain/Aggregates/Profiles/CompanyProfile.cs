using Domain.Aggregates.Internships;
using Domain.Common;
using Domain.DomainErrors;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Profiles;

public sealed class CompanyProfile : BaseAuditableEntity
{
    private CompanyProfile()
    {
    }

    private readonly List<Guid> _roadmapIds = new();
    private readonly List<Internship> _internships = new();
    private readonly List<Application> _applications = new();
    public Guid UserId { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string CompanyName { get; private set; }
    public string? Description { get; private set; }
    public string Industry { get; private set; }
    public string? YearOfEstablishment { get; private set; }
    public EgyptianTaxId TaxId { get; private set; }
    public CompanyAbout About { get; private set; }

    public Address Address { get; private set; }
    public CompanySize? Size { get; private set; }
    public IReadOnlyList<Guid> RoadmapIds => _roadmapIds.AsReadOnly();
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
        string industry)
    {
        var taxIdResult = EgyptianTaxId.Create(taxId);

        if (taxIdResult.IsFailure)
            return Result.Failure<CompanyProfile>(taxIdResult.Error);

        var addressResult = ValueObjects.Address.Create(governorate, city);
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
        string industry,
        string websiteUrl,
        string size,
        string yearOfEstablishment)
    {
        Industry = industry;
        WebsiteUrl = websiteUrl;
        Size = Enum.Parse<CompanySize>(size);
        YearOfEstablishment = yearOfEstablishment;
        return Result.Success();
    }

    public Result UpdateLogo(string logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
            return Result.Failure(CompanyErrors.InvalidLogoUrl);

        LogoUrl = logoUrl.Trim();
        return Result.Success();
    }

    public Result UpdateInfo(string name, string description)
    {
        CompanyName = name.Trim();
        Description = description?.Trim();
        return Result.Success();
    }

    public Result UpdateAddress(string governorate, string city)
    {
        var newAddress = Address.Create(governorate, city).Value;

        Address = newAddress;
        return Result.Success();
    }
}