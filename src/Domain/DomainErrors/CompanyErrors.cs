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


    public static Error AlreadyRegistered => Error.Conflict(
        "Company.AlreadyRegistered",
        "The company is already registered");

    public static Error ProfileNotFound = Error.NotFound(
        "Company.ProfileNotFound", 
        "Company profile not found");
}