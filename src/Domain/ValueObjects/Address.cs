using Domain.Common;
using Domain.Enums;
using SharedKernel;

namespace Domain.ValueObjects;

public sealed record Address : ValueObject
{
    public Governorate Governorate { get; }
    public string City { get; }
    private Address(Governorate governorate, string city)
        => (Governorate, City) = (governorate, city);

    public static Result<Address> Create(string governorate, string city)
    {
        var governorateResult = Enum.Parse<Governorate>(governorate);
        return new Address(governorateResult, city);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Governorate;
        yield return City;
    }
}