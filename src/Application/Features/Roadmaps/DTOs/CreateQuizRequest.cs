using System;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.DTOs;

public record CreateQuizRequest(
    Guid SectionId,
    string Title,
    int PassScore,
    int? TimeLimitSeconds,
    int AttemptsAllowed,
    List<QuestionRequest> Questions);

public record QuestionRequest(
    string Body,
    QuestionType Type,
    List<OptionRequest> Options);

public record OptionRequest(
    string Text,
    bool IsCorrect);

public enum QuestionType { MultipleChoice, OpenEnded }
