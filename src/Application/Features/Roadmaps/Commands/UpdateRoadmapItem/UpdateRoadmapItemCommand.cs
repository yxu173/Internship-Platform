using Application.Abstractions.Messaging;
using SharedKernel;
using System.Collections.Generic;
using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.Commands.UpdateRoadmapItem;

public record UpdateRoadmapItemCommand(
    Guid RoadmapId,
    Guid SectionId,
    Guid ItemId,         
    string Title,
    string Description,
    string Type,          
    List<ResourceLinkDto> Resources, 
    int Order
) : ICommand;