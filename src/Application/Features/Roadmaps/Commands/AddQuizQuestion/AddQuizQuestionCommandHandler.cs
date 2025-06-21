using Application.Abstractions.Messaging;
using Domain.Aggregates.Roadmaps;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Roadmaps.Commands.AddQuizQuestion;

public sealed class AddQuizQuestionCommandHandler : ICommandHandler<AddQuizQuestionCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public AddQuizQuestionCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(AddQuizQuestionCommand request, CancellationToken cancellationToken)
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

        var questionResult = section.Quiz.AddQuestion(request.Text, request.Points);
        
        if (questionResult.IsFailure)
        {
            return Result.Failure<Guid>(questionResult.Error);
        }

        await _roadmapRepository.UpdateSectionAsync(section);

        return Result.Success(questionResult.Value);
    }
} 