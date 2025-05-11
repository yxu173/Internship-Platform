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

namespace Application.Features.Roadmaps.Commands.CreateRoadmapSectionWithItems;

public sealed class CreateRoadmapSectionWithItemsCommandHandler : ICommandHandler<CreateRoadmapSectionWithItemsCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public CreateRoadmapSectionWithItemsCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(CreateRoadmapSectionWithItemsCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);
        if (roadmap == null)
        {
            return Result.Failure<Guid>(RoadmapErrors.RoadmapNotFound);
        }
        
        if (roadmap.Sections.Any(s => s.Order == request.SectionOrder))
        {
            return Result.Failure<Guid>(RoadmapErrors.DuplicateSectionOrder);
        }

        var addSectionResult = roadmap.AddSection(request.SectionTitle, request.SectionOrder);
        if (addSectionResult.IsFailure)
        {
            return Result.Failure<Guid>(addSectionResult.Error);
        }

        var sectionId = addSectionResult.Value;

        var section = roadmap.Sections.FirstOrDefault(s => s.Id == sectionId);
        if (section == null)
        {
            return Result.Failure<Guid>(RoadmapErrors.SectionNotFound);
        }

        if (request.Items != null && request.Items.Any())
        {
            foreach (var itemDto in request.Items.OrderBy(i => i.Order))
            {
                var resourceLinks = new List<ResourceLink>();
                
                if (itemDto.Resources == null || !itemDto.Resources.Any())
                {
                    return Result.Failure<Guid>(new Error(
                        "RoadmapItem.ResourcesRequired", 
                        "At least one resource is required for each roadmap item.", 
                        ErrorType.Validation));
                }
                
                foreach (var resourceDto in itemDto.Resources)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(resourceDto.Title) || 
                            string.IsNullOrWhiteSpace(resourceDto.Url) || 
                            string.IsNullOrWhiteSpace(resourceDto.Type))
                        {
                            return Result.Failure<Guid>(new Error(
                                "RoadmapItem.InvalidResource", 
                                "Resource title, URL, and type are all required.", 
                                ErrorType.Validation));
                        }
                        
                        var resourceLink = ResourceLink.Create(resourceDto.Title, resourceDto.Url, resourceDto.Type);
                        resourceLinks.Add(resourceLink);
                    }
                    catch (ArgumentException ex)
                    {
                        return Result.Failure<Guid>(new Error(
                            "RoadmapItem.InvalidResourceType", 
                            $"Invalid resource type '{resourceDto.Type}'. Error: {ex.Message}", 
                            ErrorType.Validation));
                    }
                }

                var addItemResult = section.AddItem(
                    itemDto.Title,
                    itemDto.Description,
                    itemDto.Type,
                    resourceLinks,
                    itemDto.Order
                );

                if (addItemResult.IsFailure)
                {
                    return Result.Failure<Guid>(addItemResult.Error);
                }
            }
        }

        await _roadmapRepository.Update(roadmap);
        
        return Result.Success(sectionId);
    }
}
