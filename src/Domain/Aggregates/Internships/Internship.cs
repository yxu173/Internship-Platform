using System.Net.NetworkInformation;
using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using Domain.Common;
using Domain.DomainErrors;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Internships;

public sealed class Internship : BaseAuditableEntity
{
    private readonly List<Application> _applications = new();
    public string Title { get; private set; }
    public string? About { get; private set; } = string.Empty;
    public string? KeyResponsibilities { get; private set; } = string.Empty;
    public string? Requirements { get; private set; } = string.Empty;
    public Salary Salary { get; private set; }
    public InternshipType Type { get; private set; }
    public WorkingModel WorkingModel { get; private set; }
    public DateRange Duration { get; private set; }
    public DateTime ApplicationDeadline { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CompanyProfileId { get; private set; }
    public CompanyProfile CompanyProfile { get; private set; }
    public IReadOnlyList<Application> Applications => _applications.AsReadOnly();

    private Internship()
    {
    }

    private Internship(
        Guid id,
        string title,
        string about,
        string keyResponsibilities,
        string requirements,
        Guid companyProfileId,
        InternshipType type,
        WorkingModel workingModel,
        Salary salary,
        DateRange duration,
        DateTime applicationDeadline)
    {
        CompanyProfileId = companyProfileId;
        Title = title;
        About = about;
        KeyResponsibilities = keyResponsibilities;
        Requirements = requirements;
        Salary = salary;
        WorkingModel = workingModel;
        Type = type;
        Duration = duration;
        ApplicationDeadline = applicationDeadline;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Internship> Create(
        string title,
        string about,
        string keyResponsibilities,
        string requirements,
        Guid companyProfileId,
        string type,
        string workingModel,
        decimal salary,
        string currency,
        DateRange duration,
        DateTime applicationDeadline)
    {
        var typeResult = Enum.Parse<InternshipType>(type);
        if (applicationDeadline < DateTime.UtcNow)
            return Result.Failure<Internship>(InternshipErrors.DeadlinePassed);

        if (applicationDeadline > duration.StartDate)
            return Result.Failure<Internship>(InternshipErrors.InvalidDeadline);

        var workingModelResult = Enum.Parse<WorkingModel>(workingModel);

        var salaryResult = Salary.Create(salary, currency).Value;

        return Result.Success(new Internship(
            Guid.NewGuid(),
            title,
            about,
            keyResponsibilities,
            requirements,
            companyProfileId,
            typeResult,
            workingModelResult,
            salaryResult,
            duration,
            applicationDeadline));
    }

    public Result Apply(Application application)
    {
        if (!IsActive)
            return Result.Failure(InternshipErrors.InternshipClosed);


        if (DateTime.UtcNow > ApplicationDeadline)
            return Result.Failure(InternshipErrors.DeadlinePassed);

        if (_applications.Any(a => a.StudentProfileId == application.StudentProfileId))
            return Result.Failure(InternshipErrors.DuplicateApplication);

        _applications.Add(application);
        return Result.Success();
    }

    public Result RemoveApplication(Guid applicationId)
    {
        var application = _applications.FirstOrDefault(a => a.Id == applicationId);
        if (application == null)
            return Result.Failure(InternshipErrors.ApplicationNotFound);

        _applications.Remove(application);
        return Result.Success();
    }

    public Result CloseInternship()
    {
        if (Duration.EndDate > DateTime.UtcNow)
            return Result.Failure(InternshipErrors.ActiveDurationOngoing);

        IsActive = false;
        return Result.Success();
    }

    //TODO: Add a method to update the internship details
    public Result Update(
        string title,
        string about,
        string keyResponsibilities,
        string requirements,
        string type,
        string workingModel,
        decimal salary,
        string currency,
        DateTime applicationDeadline)
    {
        if (applicationDeadline < DateTime.UtcNow)
            return Result.Failure(InternshipErrors.DeadlinePassed);

        if (applicationDeadline > Duration.StartDate)
            return Result.Failure(InternshipErrors.InvalidDeadline);

        Title = title;
        About = about;
        KeyResponsibilities = keyResponsibilities;
        Requirements = requirements;
        ApplicationDeadline = applicationDeadline;
        ModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public bool IsRemote()
    {
        return WorkingModel == WorkingModel.Remote;
    }
}