using Domain.Enums;

namespace Domain.Aggregates.Roadmaps;

public class ResourceLink
{
    private ResourceLink()
    {
        // Required by EF Core
    }
    
    // Using integer ID for compatibility with PostgreSQL IDENTITY columns
    public int Id { get; set; }
    public Guid RoadmapItemId { get; private set; }
    public string Title { get; private set; }
    public string Url { get; private set; }
    public ResourceType Type { get; private set; }

    public static ResourceLink Create(string title, string url, string type)
    {
        var typeResult = Enum.Parse<ResourceType>(type);
        return new ResourceLink
        {
            Title = title.Trim(),
            Url = url.Trim(),
            Type = typeResult
        };
    }
    
    // Method for EF Core to set the RoadmapItemId when the item is saved
    internal void SetRoadmapItem(Guid roadmapItemId)
    {
        RoadmapItemId = roadmapItemId;
    }
}
