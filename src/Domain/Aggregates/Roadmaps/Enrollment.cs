using Domain.Aggregates.Profiles;
using Domain.Common;
using Domain.DomainErrors;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class Enrollment : BaseAuditableEntity
{
    private Enrollment()
    {
    }

    private readonly List<ResourceProgress> _progress = new();
    private readonly List<SectionProgress> _sectionProgress = new();
    private readonly List<QuizAttempt> _quizAttempts = new();

    public Guid StudentId { get; private set; }
    public StudentProfile? Student { get; set; }
    public Guid RoadmapId { get; private set; }
    public Roadmap? Roadmap { get; set; }
    public DateTime EnrolledAt { get; }
    public string? PaymentProviderTransactionId { get; private set; }
    public decimal? AmountPaid { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public IReadOnlyList<ResourceProgress> Progress => _progress.AsReadOnly();
    public IReadOnlyList<SectionProgress> SectionProgress => _sectionProgress.AsReadOnly();
    public IReadOnlyList<QuizAttempt> QuizAttempts => _quizAttempts.AsReadOnly();

    private Enrollment(Guid studentId, Guid roadmapId)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        RoadmapId = roadmapId;
        EnrolledAt = DateTime.UtcNow;
        PaymentStatus = PaymentStatus.Pending;
    }

    public static Result<Enrollment> Create(Guid studentId, Guid roadmapId)
    {
        if (studentId == Guid.Empty)
            return Result.Failure<Enrollment>(RoadmapErrors.InvalidStudentId);
        if (roadmapId == Guid.Empty)
            return Result.Failure<Enrollment>(RoadmapErrors.InvalidRoadmapId);
        return Result.Success(new Enrollment(studentId, roadmapId));
    }

    public Result MarkItemCompleted(Guid itemId, Roadmap roadmap)
    {
        if (!_progress.Any(p => p.ItemId == itemId) &&
            roadmap.Sections.Any(s => s.Items.Any(i => i.Id == itemId)))
        {
            _progress.Add(new ResourceProgress(Id, itemId));
            return Result.Success();
        }

        return Result.Failure(RoadmapErrors.InvalidOrDuplicateItem);
    }

    public Result CompletePayment(string paymentProviderTransactionId, decimal amountPaid)
    {
        if (PaymentStatus == PaymentStatus.Completed)
        {
            return Result.Success();
        }

        PaymentStatus = PaymentStatus.Completed;
        PaymentProviderTransactionId = paymentProviderTransactionId;
        AmountPaid = amountPaid;
        ModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public double CalculateCompletionPercentage(Roadmap roadmap)
    {
        var totalItems = roadmap.Sections.Sum(s => s.Items.Count);
        return totalItems > 0 ? (double)_progress.Count / totalItems * 100 : 0;
    }
    
    public bool CanAccessSection(Guid sectionId, List<RoadmapSection> orderedSections)
    {
        // First section is always accessible
        var firstSection = orderedSections.FirstOrDefault();
        if (firstSection != null && firstSection.Id == sectionId)
            return true;
            
        // Find previous section
        var sectionIndex = orderedSections.FindIndex(s => s.Id == sectionId);
        if (sectionIndex <= 0) return false;
        
        var previousSection = orderedSections[sectionIndex - 1];
        
        // Check if previous section's quiz is passed or not required
        return !previousSection.HasQuiz || 
               _sectionProgress.Any(sp => sp.SectionId == previousSection.Id && sp.QuizPassed);
    }
    
    public Result<QuizAttempt> StartQuizAttempt(Guid quizId, Guid sectionId)
    {
        // Check if section exists and has a quiz
        if (_sectionProgress.Any(sp => sp.SectionId == sectionId && sp.QuizPassed))
        {
            return Result.Failure<QuizAttempt>(RoadmapErrors.QuizAlreadyPassed);
        }
        
        var attemptResult = QuizAttempt.Create(Id, quizId);
        if (attemptResult.IsFailure)
        {
            return Result.Failure<QuizAttempt>(attemptResult.Error);
        }
        
        var attempt = attemptResult.Value;
        _quizAttempts.Add(attempt);
        
        return Result.Success(attempt);
    }
    
    public Result RecordQuizResult(QuizAttempt attempt, Quiz quiz)
    {
        if (attempt.EnrollmentId != Id)
        {
            return Result.Failure(RoadmapErrors.InvalidQuizAttempt);
        }
        
        var passed = quiz.IsPassed(attempt.Score);
        attempt.SetPassed(passed);
        
        var sectionProgress = _sectionProgress.FirstOrDefault(sp => sp.SectionId == quiz.SectionId);
        if (sectionProgress == null)
        {
            sectionProgress = new SectionProgress(quiz.SectionId, Id);
            _sectionProgress.Add(sectionProgress);
        }
        
        if (passed)
        {
            sectionProgress.SetQuizResult(true);
        }
        
        ModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }
    
    public int GetSectionCompletionPercentage(Guid sectionId, RoadmapSection section)
    {
        int totalItems = section.Items.Count;
        if (totalItems == 0) return 100;
        
        int completedItems = _progress.Count(p => 
            section.Items.Any(i => i.Id == p.ItemId));
            
        return (completedItems * 100) / totalItems;
    }
}