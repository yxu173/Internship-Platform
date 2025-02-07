using SharedKernel;

namespace Domain.DomainErrors;

public static class GeneralErrors
{
    public static Error InvalidPhoneNumberFormat = Error.BadRequest(
        "General.InvalidPhoneNumberFormat",
        "The phone number format is invalid");
}