namespace Application.Features.Roadmaps.DTOs;

public sealed record AccessibleSectionDto(
    Guid Id,
    string Title,
    int Order,
    bool IsAccessible,
    bool HasQuiz,
    bool QuizPassed,
    int CompletionPercentage
); 