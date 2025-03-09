using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Aggregates.Roadmaps;
using Domain.DomainErrors;
using Domain.Repositories;
using Domain.ValueObjects;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.CreateRoadmapSection;

public sealed class CreateRoadmapSectionCommandHandler : ICommandHandler<CreateRoadmapSectionCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public CreateRoadmapSectionCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(CreateRoadmapSectionCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId);
        if (roadmap == null)
            return Result.Failure<Guid>(RoadmapErrors.RoadmapNotFound);

        var itemsResult = CreateItems(request.Items);

        if (itemsResult.IsFailure)
            return Result.Failure<Guid>(itemsResult.Error);

        var section = roadmap.AddSection(request.Title, request.Order, itemsResult.Value);

        if (section.IsFailure)
            return Result.Failure<Guid>(section.Error);

        await _roadmapRepository.Update(roadmap);
        return Result.Success(roadmap.Id);
    }

    private Result<List<RoadmapItem>> CreateItems(List<RoadmapItemDto> dtos)
    {
        var items = new List<RoadmapItem>();
        foreach (var dto in dtos)
        {
            var resources = dto.Resources.Select(r =>
                new ResourceLink(r.Title, r.Url, r.Type)).ToList();

            var itemResult = RoadmapItem.Create(
                dto.Title,
                dto.Description,
                dto.Type,
                resources,
                dto.Order);

            if (itemResult.IsFailure)
                return Result.Failure<List<RoadmapItem>>(itemResult.Error);

            items.Add(itemResult.Value);
        }

        return items;
    }
}