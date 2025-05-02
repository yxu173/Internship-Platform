using Application.Abstractions.Messaging;

namespace Application.Features.Roadmaps.Commands.EnrollStudent;

public record EnrollStudentCommand(
    Guid UserId, 
    Guid RoadmapId 
) : ICommand;