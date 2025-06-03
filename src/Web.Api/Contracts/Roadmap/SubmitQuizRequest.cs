using Application.Features.Roadmaps.DTOs;

namespace Web.Api.Contracts.Roadmap;

public sealed record SubmitQuizRequest(
    List<QuizAnswerDto> Answers
); 