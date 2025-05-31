using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Commands.CreateRoadmapSection;

public record CreateRoadmapSectionCommand(
    Guid RoadmapId,
    string Title,
    int Order
) : ICommand<Guid>;