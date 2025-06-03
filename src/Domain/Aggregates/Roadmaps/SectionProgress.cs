using Domain.Common;

namespace Domain.Aggregates.Roadmaps;

public sealed record SectionProgress : ValueObject
{
    public Guid SectionId { get; }
    public Guid EnrollmentId { get; }
    public bool QuizPassed { get; private set; }
    
    public SectionProgress(Guid sectionId, Guid enrollmentId)
    {
        SectionId = sectionId;
        EnrollmentId = enrollmentId;
        QuizPassed = false;
    }
    
    public void SetQuizResult(bool passed)
    {
        QuizPassed = passed;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SectionId;
        yield return EnrollmentId;
    }
} 