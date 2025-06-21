using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class Quiz : BaseEntity
{
    private Quiz()
    {
    }

    private readonly List<QuizQuestion> _questions = new();
    
    public Guid SectionId { get; private set; }
    public RoadmapSection Section { get; private set; }
    public int PassingScore { get; private set; }
    public bool IsRequired { get; private set; }
    public IReadOnlyList<QuizQuestion> Questions => _questions.AsReadOnly();
    
    public Quiz(Guid sectionId, int passingScore, bool isRequired)
    {
        SectionId = sectionId;
        PassingScore = passingScore;
        IsRequired = isRequired;
    }

    public static Result<Quiz> Create(Guid sectionId, int passingScore, bool isRequired)
    {
        if (sectionId == Guid.Empty)
            return Result.Failure<Quiz>(RoadmapErrors.SectionNotFound);
            
        if (passingScore < 0 || passingScore > 100)
            return Result.Failure<Quiz>(RoadmapErrors.InvalidQuizParams);
            
        return Result.Success(new Quiz(sectionId, passingScore, isRequired));
    }
    
    public Result<QuizQuestion> AddQuestion(string text, int points)
    {
        var question = new QuizQuestion(text, points, Id);
        _questions.Add(question);
        return Result.Success(question);
    }
    
    public Result UpdateQuestion(Guid questionId, string text, int points)
    {
        var question = _questions.FirstOrDefault(q => q.Id == questionId);
        if (question is null)
            return Result.Failure(RoadmapErrors.QuestionNotFound);
            
        question.UpdateDetails(text, points);
        return Result.Success();
    }
    
    public Result RemoveQuestion(Guid questionId)
    {
        var question = _questions.FirstOrDefault(q => q.Id == questionId);
        if (question is null)
            return Result.Success();
            
        _questions.Remove(question);
        return Result.Success();
    }
    
    public int CalculateTotalPoints()
    {
        return _questions.Sum(q => q.Points);
    }
    
    public bool IsPassed(int score)
    {
        if (_questions.Count == 0) return true;
        
        var totalPoints = CalculateTotalPoints();
        if (totalPoints == 0) return true;
        
        var percentage = (score * 100) / totalPoints;
        return percentage >= PassingScore;
    }
} 