using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Aggregates.Roadmaps;

public sealed class RoadmapItem : BaseEntity
{
    public string Title { get; }
    public string Description { get; }
    public ResourceType Type { get; }
    public List<ResourceLink> Resources { get; }
    public int Order { get; }
    public bool IsPremium { get; }

    public RoadmapItem(
        string title,
        string description,
        ResourceType type,
        List<ResourceLink> resources,
        int order,
        bool isPremium)
    {
        Title = title.Trim();
        Description = description?.Trim();
        Type = type;
        Resources = resources;
        Order = order;
        IsPremium = isPremium;
    }

    public static RoadmapItem CreateComparisonItem(
        string primaryTool,
        List<ResourceLink> primaryResources,
        string secondaryTool,
        List<ResourceLink> secondaryResources,
        int order,
        bool isPremium)
    {
        return new RoadmapItem(
            title: $"Compare {primaryTool} vs {secondaryTool}",
            description: $"Detailed comparison between {primaryTool} and {secondaryTool}",
            type: ResourceType.ToolComparison,
            resources: primaryResources.Concat(secondaryResources).ToList(),
            order: order,
            isPremium: isPremium
        );
    }
}