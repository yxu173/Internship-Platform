using System.Text.RegularExpressions;
using Domain.Common;
using SharedKernel;

namespace Domain.ValueObjects;

public sealed record EgyptianTaxId : ValueObject
{
    public string Value { get; }
    
    private EgyptianTaxId() { }
    private EgyptianTaxId(string value) => Value = value;

    public static Result<EgyptianTaxId> Create(string taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId))
            return Result.Failure<EgyptianTaxId>(DomainErrors.CompanyErrors.TaxIdRequired);

        if (!Regex.IsMatch(taxId, @"^3\d{13}$"))
            return Result.Failure<EgyptianTaxId>(DomainErrors.CompanyErrors.InvalidTaxIdFormat);

        return new EgyptianTaxId(taxId);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}