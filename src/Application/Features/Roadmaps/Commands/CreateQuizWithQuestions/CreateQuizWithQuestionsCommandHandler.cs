using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Aggregates.Roadmaps;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Roadmaps.Commands.CreateQuizWithQuestions;

public sealed class CreateQuizWithQuestionsCommandHandler : ICommandHandler<CreateQuizWithQuestionsCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IApplicationDbContext _context;

    public CreateQuizWithQuestionsCommandHandler(IRoadmapRepository roadmapRepository, IApplicationDbContext context)
    {
        _roadmapRepository = roadmapRepository;
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateQuizWithQuestionsCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);
        if (roadmap == null)
        {
            return Result.Failure<Guid>(RoadmapErrors.RoadmapNotFound);
        }

        var section = roadmap.Sections.FirstOrDefault(s => s.Id == request.SectionId);
        if (section == null)
        {
            return Result.Failure<Guid>(RoadmapErrors.SectionNotFound);
        }

        var addQuizResult = section.AddQuiz(request.PassingScore, request.IsRequired);
        if (addQuizResult.IsFailure)
        {
            return Result.Failure<Guid>(addQuizResult.Error);
        }

        var quiz = section.Quiz;

        // Build questions & options first
        foreach (var questionDto in request.Questions)
        {
            var addQuestionResult = quiz.AddQuestion(questionDto.Text, questionDto.Points);
            if (addQuestionResult.IsFailure)
            {
                return Result.Failure<Guid>(addQuestionResult.Error);
            }
            var questionId = addQuestionResult.Value; 
            var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId); 
            if (question == null)
            {
                return Result.Failure<Guid>(new Error("QuizQuestionNotFound", "Question not found after addition", ErrorType.Validation));
            }

            foreach (var optionDto in questionDto.Options)
            {
                var addOptionResult = question.AddOption(optionDto.Text, optionDto.IsCorrect);
                if (addOptionResult.IsFailure)
                {
                    return Result.Failure<Guid>(addOptionResult.Error);
                }
            }
        }

        // Save the entire roadmap with all its sections and quiz
        await _roadmapRepository.Update(roadmap);

        return Result.Success(quiz.Id);
    }
}