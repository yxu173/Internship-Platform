using Domain.Common;
using Domain.DomainErrors;
using Domain.Enums;
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
        Order = order;
        
        // First clear any existing resources
        _resourceLinks.Clear();
        
        // Add each resource and set its RoadmapItemId
        foreach (var resource in resources)
        {
            resource.SetRoadmapItem(Id); // Use the SetRoadmapItem method we added
            _resourceLinks.Add(resource);
        }
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
            
        // Create the item with the resources attached
        var item = new RoadmapItem(title, description, typeResult, resources, order);
        
        // Setting RoadmapItemId happens in the constructor now
        
        return item;
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
        
        // Clear existing resources
        _resourceLinks.Clear();
        
        // Add new resources and set their RoadmapItemId property
        foreach (var resource in resources)
        {
            // Set RoadmapItemId using the method we added
            resource.SetRoadmapItem(Id);
            _resourceLinks.Add(resource);
        }

        return Result.Success();
    }
}