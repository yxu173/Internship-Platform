using System.Collections.Generic;

namespace Web.Api.Contracts.Roadmap;

public record CreateRoadmapSectionWithItemsRequest(
    string SectionTitle,
    int SectionOrder,
    List<CreateRoadmapItemRequest> Items
);
