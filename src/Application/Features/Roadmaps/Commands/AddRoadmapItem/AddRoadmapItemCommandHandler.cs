using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps;
using Domain.ValueObjects; // Needed for ResourceLink
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Roadmaps.Commands.AddRoadmapItem;

internal sealed class AddRoadmapItemCommandHandler : ICommandHandler<AddRoadmapItemCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public AddRoadmapItemCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(AddRoadmapItemCommand request, CancellationToken cancellationToken)
    {
        var section = await _roadmapRepository.GetSectionByIdAsync(request.SectionId, includeItems: true);
        if (section is null)
        {
            return Result.Failure<Guid>(RoadmapErrors.SectionNotFound);
        }

        if (section.RoadmapId != request.RoadmapId)
        {
             return Result.Failure<Guid>(new Error("AddRoadmapItem.Mismatch", "Section does not belong to the specified roadmap.", ErrorType.Validation));
        }

        // TODO: Add authorization check - does the user own the parent roadmap?

        var resourceLinks = new List<ResourceLink>();
        foreach (var resourceDto in request.Resources)
        {
            try
            {
                var resourceLink = new ResourceLink(resourceDto.Title, resourceDto.Url, resourceDto.Type);
                resourceLinks.Add(resourceLink);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<Guid>(new Error("RoadmapItem.InvalidResourceType", $"Invalid resource type '{resourceDto.Type}'. Error: {ex.Message}", ErrorType.Validation));
            }
        }

        var addItemResult = section.AddItem(
            request.Title,
            request.Description,
            request.Type,
            resourceLinks,
            request.Order
        );

        if (addItemResult.IsFailure)
        {
            return addItemResult; 
        }

        await _roadmapRepository.UpdateSectionAsync(section); 

        return addItemResult; 
    }
} 