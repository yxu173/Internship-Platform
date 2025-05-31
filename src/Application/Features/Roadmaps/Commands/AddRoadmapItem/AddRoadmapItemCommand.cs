using Application.Abstractions.Messaging;
using SharedKernel;
using System.Collections.Generic;
using Application.Features.Roadmaps.DTOs; 

namespace Application.Features.Roadmaps.Commands.AddRoadmapItem;

public record AddRoadmapItemCommand(
    Guid RoadmapId,       
    Guid SectionId,      
    string Title,
    string Description,
    string Type,          
    List<ResourceLinkDto> Resources, 
    int Order
) : ICommand<Guid>; 