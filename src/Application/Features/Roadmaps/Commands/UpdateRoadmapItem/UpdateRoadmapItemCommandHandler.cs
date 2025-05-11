using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps;
using Domain.ValueObjects;
using System.Linq;
using System.Collections.Generic;
using ResourceLink = Domain.Aggregates.Roadmaps.ResourceLink;

namespace Application.Features.Roadmaps.Commands.UpdateRoadmapItem;

internal sealed class UpdateRoadmapItemCommandHandler : ICommandHandler<UpdateRoadmapItemCommand>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public UpdateRoadmapItemCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result> Handle(UpdateRoadmapItemCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);
        if (roadmap is null)
        {
            return Result.Failure(RoadmapErrors.RoadmapNotFound);
        }

        // TODO: Add authorization check

        var section = roadmap.Sections.FirstOrDefault(s => s.Id == request.SectionId);
        if (section is null)
        {
            return Result.Failure(RoadmapErrors.SectionNotFound);
        }

        var resourceLinks = new List<ResourceLink>();
        foreach (var resourceDto in request.Resources)
        {
            try
            {
                var resourceLink = ResourceLink.Create(resourceDto.Title, resourceDto.Url, resourceDto.Type);
                resourceLinks.Add(resourceLink);
            }
            catch (ArgumentException ex) 
            {
                return Result.Failure(new Error("RoadmapItem.InvalidResourceType", $"Invalid resource type '{resourceDto.Type}'. Error: {ex.Message}", ErrorType.Validation));
            }
        }

        var updateResult = section.UpdateItem(
            request.ItemId,
            request.Title,
            request.Description,
            request.Type,
            resourceLinks,
            request.Order
        );

        if (updateResult.IsFailure)
        {
            return updateResult; 
        }

        await _roadmapRepository.Update(roadmap);

        return Result.Success();
    }
} 