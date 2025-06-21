namespace Application.Features.Roadmaps.DTOs;

public record GetQuizAttemptResponse(
    Guid Id,
    Guid EnrollmentId,
    Guid QuizId,
    int Score,
    int TotalPoints,
    int PassingScore,
    bool Passed,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    IReadOnlyList<QuestionResultDto> Results
);

public sealed record GetQuizAnswerResponse(
    Guid QuestionId,
    Guid SelectedOptionId,
    bool IsCorrect
);