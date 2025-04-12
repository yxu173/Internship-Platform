using System.Collections.Generic;

namespace Web.Api.Contracts.Roadmap;

public record UpdateRoadmapItemRequest(
    string Title,
    string Description,
    string Type, 
    List<CreateResourceLinkRequest> Resources,
    int Order
); 