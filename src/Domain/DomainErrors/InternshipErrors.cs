using SharedKernel;

namespace Domain.DomainErrors;

public static class InternshipErrors
{
    public static Error InvalidDateRange => Error.Validation(
        "Internship.InvalidDateRange",
        "End date must be after start date");

    public static Error MaxDurationExceeded => Error.Validation(
        "Internship.MaxDurationExceeded",
        "Internship duration cannot exceed 6 months");

    public static Error DeadlinePassed => Error.Validation(
        "Internship.DeadlinePassed",
        "Application deadline has passed");

    public static Error InvalidDeadline => Error.Validation(
        "Internship.InvalidDeadline",
        "Deadline must be before internship start date");

    public static Error DuplicateApplication => Error.Conflict(
        "Internship.DuplicateApplication",
        "Student has already applied to this internship");

    public static Error ActiveDurationOngoing => Error.Conflict(
        "Internship.ActiveDurationOngoing",
        "Cannot close internship before end date");

    public static Error ApplicationWithdrawn => Error.Conflict(
        "Internship.ApplicationWithdrawn",
        "Withdrawn applications cannot be modified");

    public static Error InternshipClosed => Error.Failure(
        "Internship.InternshipClosed",
        "Internship Closed"
    );
    public static Error NotFound => Error.NotFound(
        "Internship.NotFound",
        "Internship not found"
    );
    public static Error ApplicationNotFound => Error.NotFound(
        "Internship.ApplicationNotFound",
        "Application not found"
    );

    public static Error NotApplicationOwner => Error.Conflict(
        " Internship.NotApplicationOwner",
        "You are not the owner of this application"
    );
    public static Error InvalidSalary => Error.Validation(
        "Internship.InvalidSalary",
        "Salary must be greater than or equal to 0"
    );
    public static Error InvalidCurrency => Error.Validation(
        "Internship.InvalidCurrency",
        "Currency cannot be empty"
    );
}