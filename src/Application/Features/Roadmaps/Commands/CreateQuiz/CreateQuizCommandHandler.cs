using Application.Abstractions.Messaging;
using Domain.Aggregates.Roadmaps;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.CreateQuiz;

public sealed class CreateQuizCommandHandler : ICommandHandler<CreateQuizCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public CreateQuizCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId,true);
        if (roadmap == null)
        {
            return Result.Failure<Guid>(RoadmapErrors.RoadmapNotFound);
        }

        var section = roadmap.Sections.FirstOrDefault(s => s.Id == request.SectionId);
        if (section == null)
        {
            return Result.Failure<Guid>(RoadmapErrors.SectionNotFound);
        }

        var result = section.AddQuiz(request.PassingScore, request.IsRequired);
        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        await _roadmapRepository.Update(roadmap);

        return Result.Success(section.Quiz.Id);
    }
} 