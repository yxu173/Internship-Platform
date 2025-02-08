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
        public string Description { get; private set; }
        public Guid CompanyId { get; }
        public InternshipType Type { get; private set; }
        public DateRange Duration { get; private set; }
        public DateTime ApplicationDeadline { get; private set; }
        public bool IsActive { get; private set; }
        public IReadOnlyList<Application> Applications => _applications.AsReadOnly();
        private Internship(){}
        private Internship(
            Guid id,
            string title,
            string description,
            Guid companyId,
            InternshipType type,
            DateRange duration,
            DateTime applicationDeadline)
        {
            Title = title;
            Description = description;
            CompanyId = companyId;
            Type = type;
            Duration = duration;
            ApplicationDeadline = applicationDeadline;
            IsActive = true;
        }

        public static Result<Internship> Create(
            string title,
            string description,
            Guid companyId,
            InternshipType type,
            DateRange duration,
            DateTime applicationDeadline)
        {
            if (applicationDeadline < DateTime.UtcNow)
                return Result.Failure<Internship>(InternshipErrors.DeadlinePassed);

            if (applicationDeadline > duration.StartDate)
                return Result.Failure<Internship>(InternshipErrors.InvalidDeadline);

            return new Internship(
                Guid.NewGuid(),
                title,
                description,
                companyId,
                type,
                duration,
                applicationDeadline);
        }

    public Result Apply(Application application)
    {
        if (!IsActive)
            return Result.Failure(InternshipErrors.InternshipClosed);
        

        if (DateTime.UtcNow > ApplicationDeadline)
            return Result.Failure(InternshipErrors.DeadlinePassed);

        if (_applications.Any(a => a.StudentId == application.StudentId))
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
}