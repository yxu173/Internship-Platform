using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;
using System;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Commands.CreateRoadmapSectionWithItems;

public record CreateRoadmapSectionWithItemsCommand(
    Guid RoadmapId,
    string SectionTitle,
    int SectionOrder,
    List<RoadmapItemDto> Items
) : ICommand<Guid>;

public record RoadmapItemDto(
    string Title,
    string Description,
    string Type,
    List<ResourceLinkDto> Resources,
    int Order
);
