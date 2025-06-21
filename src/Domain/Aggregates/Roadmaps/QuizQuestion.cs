using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class QuizQuestion : BaseEntity
{
    private QuizQuestion()
    {
    }

    private readonly List<QuizOption> _options = new();
    
    public Guid QuizId { get; private set; }
    public string Text { get; private set; }
    public int Points { get; private set; }
    public IReadOnlyList<QuizOption> Options => _options.AsReadOnly();
    
    public QuizQuestion(string text, int points, Guid quizId)
    {
        Text = text.Trim();
        Points = points;
        QuizId = quizId;
    }
    
    public void UpdateDetails(string text, int points)
    {
        Text = text.Trim();
        Points = points;
    }
    
    public Result<Guid> AddOption(string text, bool isCorrect)
    {
        var option = new QuizOption(text, isCorrect);
        _options.Add(option);
        return Result.Success(option.Id);
    }
    
    public Result UpdateOption(Guid optionId, string text, bool isCorrect)
    {
        var option = _options.FirstOrDefault(o => o.Id == optionId);
        if (option is null)
            return Result.Failure(RoadmapErrors.OptionNotFound);
            
        option.UpdateDetails(text, isCorrect);
        return Result.Success();
    }
    
    public Result RemoveOption(Guid optionId)
    {
        var option = _options.FirstOrDefault(o => o.Id == optionId);
        if (option is null)
            return Result.Success();
            
        _options.Remove(option);
        return Result.Success();
    }
    
    public bool HasCorrectOption()
    {
        return _options.Any(o => o.IsCorrect);
    }
    
    public bool IsCorrectOption(Guid optionId)
    {
        var option = _options.FirstOrDefault(o => o.Id == optionId);
        return option?.IsCorrect ?? false;
    }
}