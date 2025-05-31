using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.ValueObjects;

public sealed record Salary : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Salary(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Salary> Create(decimal amount, string currency)
    {
        if (amount < 0)
            return Result.Failure<Salary>(InternshipErrors.InvalidSalary);
        
        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<Salary>(InternshipErrors.InvalidCurrency);

        return new Salary(amount, currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}