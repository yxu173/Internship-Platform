using Domain.Common;

namespace Domain.Aggregates.Roadmaps;

public sealed record QuizAnswer : ValueObject
{
    public Guid QuestionId { get; }
    public Guid SelectedOptionId { get; }
    public bool IsCorrect { get; }
    
    public QuizAnswer(Guid questionId, Guid selectedOptionId, bool isCorrect)
    {
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return QuestionId;
        yield return SelectedOptionId;
        yield return IsCorrect;
    }
} 