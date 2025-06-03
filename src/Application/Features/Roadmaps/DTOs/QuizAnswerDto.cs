namespace Application.Features.Roadmaps.DTOs;

public sealed record QuizAnswerDto(
    Guid QuestionId,
    Guid SelectedOptionId
); 