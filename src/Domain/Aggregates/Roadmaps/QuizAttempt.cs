using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class QuizAttempt : BaseAuditableEntity
{
    private QuizAttempt()
    {
    }

    private readonly List<QuizAnswer> _answers = new();
    
    public Guid EnrollmentId { get; private set; }
    public Guid QuizId { get; private set; }
    public int Score { get; private set; }
    public bool Passed { get; private set; }
    public IReadOnlyList<QuizAnswer> Answers => _answers.AsReadOnly();
    
    private QuizAttempt(Guid enrollmentId, Guid quizId)
    {
        EnrollmentId = enrollmentId;
        QuizId = quizId;
        Score = 0;
        Passed = false;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static Result<QuizAttempt> Create(Guid enrollmentId, Guid quizId)
    {
        if (enrollmentId == Guid.Empty)
            return Result.Failure<QuizAttempt>(RoadmapErrors.InvalidStudentId);
            
        if (quizId == Guid.Empty)
            return Result.Failure<QuizAttempt>(RoadmapErrors.QuizNotFound);
            
        return Result.Success(new QuizAttempt(enrollmentId, quizId));
    }
    
    public void AddAnswer(Guid questionId, Guid selectedOptionId, bool isCorrect, int questionPoints)
    {
        var answer = new QuizAnswer(questionId, selectedOptionId, isCorrect);
        _answers.Add(answer);
        
        if (isCorrect)
        {
            Score += questionPoints;
        }
    }
    
    public void SetPassed(bool passed)
    {
        Passed = passed;
        ModifiedAt = DateTime.UtcNow;
    }
} 