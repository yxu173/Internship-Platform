using System.Collections.Generic;

namespace Web.Api.Contracts.Roadmap;

public record CreateRoadmapItemRequest(
    string Title,
    List<CreateResourceLinkRequest> Resources,
    int Order
); 