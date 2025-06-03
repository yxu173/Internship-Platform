namespace Application.Features.Roadmaps.DTOs;

public sealed record QuizResultDto(
    Guid QuizId,
    Guid AttemptId,
    int Score,
    int TotalPoints,
    int PassingScore,
    bool Passed,
    bool NextSectionUnlocked,
    List<QuestionResultDto> QuestionResults
);

public sealed record QuestionResultDto(
    Guid QuestionId,
    string QuestionText,
    int QuestionPoints,
    bool IsCorrect,
    Guid SelectedOptionId,
    List<OptionDto> Options
);

public sealed record OptionDto(
    Guid OptionId, 
    string Text, 
    bool IsCorrect
); 