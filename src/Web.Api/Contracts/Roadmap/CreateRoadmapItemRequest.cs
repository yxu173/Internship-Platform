using System.Collections.Generic;

namespace Web.Api.Contracts.Roadmap;

public record CreateRoadmapItemRequest(
    string Title,
    string Description,
    string Type,
    List<CreateResourceLinkRequest> Resources,
    int Order
); 