using Domain.Common;
using SharedKernel;

namespace Domain.ValueObjects;

public sealed record Year : ValueObject
{
    public int Value { get; }

    private Year(int value) => Value = value;

    public static Result<Year> Create(int year)
    {
        if (year < 2000 || year > DateTime.Now.Year + 5)
            return Result.Failure<Year>(DomainErrors.StudentErrors.InvalidGraduationYear);

        return new Year(year);
    }

    public static implicit operator int(Year year) => year.Value;
    public static explicit operator Year(int year) => new(year);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}