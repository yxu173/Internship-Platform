using Domain.Common;

namespace Domain.Aggregates.Roadmaps;

public sealed class QuizOption : BaseEntity
{
    private QuizOption()
    {
    }
    
    public string Text { get; private set; }
    public bool IsCorrect { get; private set; }
    
    public QuizOption(string text, bool isCorrect)
    {
        Text = text.Trim();
        IsCorrect = isCorrect;
    }
    
    public void UpdateDetails(string text, bool isCorrect)
    {
        Text = text.Trim();
        IsCorrect = isCorrect;
    }
} 