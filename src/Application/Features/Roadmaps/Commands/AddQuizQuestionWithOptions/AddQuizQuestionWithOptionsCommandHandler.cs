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

namespace Application.Features.Roadmaps.Commands.AddQuizQuestionWithOptions
{
    public sealed class AddQuizQuestionWithOptionsCommandHandler : ICommandHandler<AddQuizQuestionWithOptionsCommand, Guid>
    {
        private readonly IRoadmapRepository _roadmapRepository;

        public AddQuizQuestionWithOptionsCommandHandler(IRoadmapRepository roadmapRepository)
        {
            _roadmapRepository = roadmapRepository;
        }

        public async Task<Result<Guid>> Handle(AddQuizQuestionWithOptionsCommand request, CancellationToken cancellationToken)
        {
            var section = await _roadmapRepository.GetSectionByIdAsync(request.SectionId, includeQuiz: true);
            
            if (section == null)
            {
                return Result.Failure<Guid>(RoadmapErrors.SectionNotFound);
            }
            
            if (section.RoadmapId != request.RoadmapId)
            {
                return Result.Failure<Guid>(RoadmapErrors.SectionNotFound);
            }

            if (section.Quiz == null || section.Quiz.Id != request.QuizId)
            {
                return Result.Failure<Guid>(RoadmapErrors.QuizNotFound);
            }
            
            var questionResult = section.Quiz.AddQuestion(request.QuestionText, request.QuestionPoints);
            if (questionResult.IsFailure)
            {
                return Result.Failure<Guid>(questionResult.Error);
            }
            
            var questionId = questionResult.Value;
            var question = section.Quiz.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
            {
                return Result.Failure<Guid>(Error.NotFound("Question not found after addition"
                ," Question with ID: " + questionId));
            }
            
            foreach (var option in request.Options)
            {
                var optionResult = question.AddOption(option.Text, option.IsCorrect);
                if (optionResult.IsFailure)
                {
                    return Result.Failure<Guid>(optionResult.Error);
                }
            }
            
            await _roadmapRepository.UpdateSectionAsync(section);
            
            return Result.Success(questionId);
        }
    }
}
