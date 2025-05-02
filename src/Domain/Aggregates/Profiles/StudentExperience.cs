using Domain.Common;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Profiles;

public sealed class StudentExperience : BaseEntity
{
    private StudentExperience()
    {
    }

    public Guid StudentProfileId { get; private set; }
    public string JobTitle { get; private set; }
    public string CompanyName { get; private set; }
    public DateRange DateRange { get; private set; }
    public StudentProfile StudentProfile { get; private set; }

    private StudentExperience(
        Guid studentProfileId,
        string jobTitle,
        string companyName,
        DateTime startDate,
        DateTime? endDate)
    {
        StudentProfileId = studentProfileId;
        JobTitle = jobTitle;
        CompanyName = companyName;
        DateRange = DateRange.Create(startDate, endDate ?? DateTime.UtcNow).Value;
    }

    public static Result<StudentExperience> Create(
        Guid studentProfileId,
        string jobTitle,
        string companyName,
        DateTime startDate,
        DateTime? endDate)
    {
        return Result.Success(new StudentExperience(
            studentProfileId,
            jobTitle,
            companyName,
            startDate,
            endDate));
    }
}