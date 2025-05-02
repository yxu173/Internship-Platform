using Domain.Common;
using Domain.DomainErrors;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class RoadmapItem : BaseEntity
{
    private RoadmapItem()
    {
    }

    private readonly List<ResourceLink> _resourceLinks = new();
    
    public Guid SectionId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public ResourceType Type { get; private set; }
    public IReadOnlyList<ResourceLink> Resources => _resourceLinks.AsReadOnly();
    public int Order { get; }

    public RoadmapItem(
        string title,
        string description,
        ResourceType type,
        List<ResourceLink> resources,
        int order)
    {
        Title = title.Trim();
        Description = description?.Trim();
        Type = type;
        _resourceLinks = resources;
        Order = order;
    }

    public static Result<RoadmapItem> Create(
        string title,
        string description,
        string type,
        List<ResourceLink> resources,
        int order)
    {
        var typeResult = Enum.Parse<ResourceType>(type);
        if (resources == null || !resources.Any())
            return Result.Failure<RoadmapItem>(RoadmapErrors.ResourcesRequired);
        return new RoadmapItem(title, description, typeResult, resources, order);
    }

    internal Result UpdateDetails(string title, string description, string type, List<ResourceLink> resources, int order)
    {
        // Validate inputs
        if (resources == null || !resources.Any())
        {
            return Result.Failure(RoadmapErrors.ResourcesRequired);
        }
        
        ResourceType typeResult;
        try
        {
            typeResult = Enum.Parse<ResourceType>(type);
        }
        catch (ArgumentException)
        {
            return Result.Failure(RoadmapErrors.InvalidResourceType); 
        }

        Title = title.Trim();
        Description = description?.Trim();
        Type = typeResult;
        _resourceLinks.Clear();
        _resourceLinks.AddRange(resources); 

        return Result.Success();
    }
}