using Domain.Common;
using Domain.Enums;
using SharedKernel;

namespace Domain.ValueObjects;

public sealed record Address : ValueObject
{
    public Governorate Governorate { get; }
    public string City { get; }
    public string Street { get; }
    private Address(Governorate governorate, string city, string street)
        => (Governorate, City, Street) = (governorate, city, street);

    public static Result<Address> Create(string governorate, string city, string street)
    {
        var governorateResult = Enum.Parse<Governorate>(governorate);
        return new Address(governorateResult, city, street);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Governorate;
        yield return City;
        yield return Street;
    }
}