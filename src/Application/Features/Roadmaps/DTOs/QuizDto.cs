namespace Application.Features.Roadmaps.DTOs;

public sealed record QuizDto(
    Guid Id,
    Guid SectionId,
    string SectionTitle,
    int PassingScore,
    bool IsRequired,
    List<QuizQuestionDto> Questions
);

public sealed record QuizQuestionDto(
    Guid Id,
    string Text,
    int Points,
    List<QuizOptionDto> Options
);

public sealed record QuizOptionDto(
    Guid Id,
    string Text,
    bool IsCorrect
); 