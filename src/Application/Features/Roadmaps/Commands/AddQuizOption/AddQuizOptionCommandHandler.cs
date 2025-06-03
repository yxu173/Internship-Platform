using Application.Abstractions.Messaging;
using Domain.Aggregates.Roadmaps;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.AddQuizOption;

public sealed class AddQuizOptionCommandHandler : ICommandHandler<AddQuizOptionCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public AddQuizOptionCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(AddQuizOptionCommand request, CancellationToken cancellationToken)
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

        var question = section.Quiz.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
        if (question == null)
        {
            return Result.Failure<Guid>(RoadmapErrors.QuestionNotFound);
        }

        var optionResult = question.AddOption(request.Text, request.IsCorrect);
        if (optionResult.IsFailure)
        {
            return Result.Failure<Guid>(optionResult.Error);
        }

        await _roadmapRepository.UpdateSectionAsync(section);

        return Result.Success(optionResult.Value);
    }
} 