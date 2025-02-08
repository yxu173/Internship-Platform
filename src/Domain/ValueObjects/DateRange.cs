using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.ValueObjects;

public sealed record DateRange : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    private DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<DateRange> Create(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            return Result.Failure<DateRange>(InternshipErrors.InvalidDateRange);

        if (endDate > startDate.AddMonths(6))
            return Result.Failure<DateRange>(InternshipErrors.MaxDurationExceeded);

        return new DateRange(startDate, endDate);
    }

    public int DurationInWeeks => (int)(EndDate - StartDate).TotalDays / 7;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;

    }
}