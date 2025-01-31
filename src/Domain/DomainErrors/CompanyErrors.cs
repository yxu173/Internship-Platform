using SharedKernel;

namespace Domain.DomainErrors;

public static class CompanyErrors
{
    public static Error TaxIdRequired => Error.BadRequest(
        "Company.TaxIdRequired",
        "The tax id is required");
    
    public static Error InvalidTaxIdFormat => Error.BadRequest(
        "Company.InvalidTaxIdFormat",
        "The tax id format is invalid");
}