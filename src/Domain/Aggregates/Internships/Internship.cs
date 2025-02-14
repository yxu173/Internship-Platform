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
       // TODO: Internship model -   
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid CompanyProfileId { get; private set; }
        public InternshipType Type { get; private set; }
        public DateRange Duration { get; private set; }
        public DateTime ApplicationDeadline { get; private set; }
        public bool IsActive { get; private set; }
        public CompanyProfile CompanyProfile { get; private set; }
        public IReadOnlyList<Application> Applications => _applications.AsReadOnly();
        private Internship(){}
        private Internship(
            Guid id,
            string title,
            string description,
            Guid companyProfileId ,
            InternshipType type,
            DateRange duration,
            DateTime applicationDeadline)
        {
            Title = title;
            Description = description;
            CompanyProfileId = companyProfileId;
            Type = type;
            Duration = duration;
            ApplicationDeadline = applicationDeadline;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Internship> Create(
            string title,
            string description,
            Guid companyProfileId,
            string type,
            DateRange duration,
            DateTime applicationDeadline)
        {
            var typeResult = Enum.Parse<InternshipType>(type);            
            if (applicationDeadline < DateTime.UtcNow)
                return Result.Failure<Internship>(InternshipErrors.DeadlinePassed);

            if (applicationDeadline > duration.StartDate)
                return Result.Failure<Internship>(InternshipErrors.InvalidDeadline);

            return new Internship(
                Guid.NewGuid(),
                title,
                description,
                companyProfileId,
                typeResult,
                duration,
                applicationDeadline);
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

        public Result CloseInternship()
        {
            if (Duration.EndDate > DateTime.UtcNow)
                return Result.Failure(InternshipErrors.ActiveDurationOngoing);

            IsActive = false;
            return Result.Success();
        }

    public Result Update(
        string title,
        string description,
        DateTime applicationDeadline)
    {
        if (applicationDeadline < DateTime.UtcNow)
            return Result.Failure(InternshipErrors.DeadlinePassed);

        if (applicationDeadline > Duration.StartDate)
            return Result.Failure(InternshipErrors.InvalidDeadline);

        Title = title;
        Description = description;
        ApplicationDeadline = applicationDeadline;
        ModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
