using System.Text.RegularExpressions;
using Domain.Common;
using SharedKernel;

namespace Domain.ValueObjects;

public sealed record PhoneNumber : ValueObject<string>
{
    private PhoneNumber(string value) : base(value)
    {
    }

    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        if (!Regex.IsMatch(phoneNumber, @"^\+20(10|11|12|15)\d{8}$"))
            return Result.Failure<PhoneNumber>(DomainErrors.GeneralErrors.InvalidPhoneNumberFormat);

        return new PhoneNumber(phoneNumber);
    }
}