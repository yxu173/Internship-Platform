using System;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.DTOs;

public record SubmitQuizRequest(
    Guid EnrollmentId,
    Guid QuizId,
    List<AnswerSubmit> Answers);

public record AnswerSubmit(
    Guid QuestionId,
    List<Guid> SelectedOptionIds,
    string? FreeText);
