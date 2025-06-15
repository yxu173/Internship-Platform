using System;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Commands.CreateQuizWithQuestions;

public sealed class CreateQuizWithQuestionsRequest
{
    public int PassingScore { get; set; }
    public bool IsRequired { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}   

public record QuestionDto(
     string Text,  
    int Points,
    List<OptionDto> Options
);

public class OptionDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
