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
    public IReadOnlyList<ResourceLink> Resources => _resourceLinks.AsReadOnly();
    public int Order { get; }

    public RoadmapItem(
        string title,
        List<ResourceLink> resources,
        int order)
    {
        Title = title.Trim();
        Order = order;
        
        _resourceLinks.Clear();
        
        foreach (var resource in resources)
        {
            resource.SetRoadmapItem(Id); 
            _resourceLinks.Add(resource);
        }
    }

    public static Result<RoadmapItem> Create(
        string title,
        List<ResourceLink> resources,
        int order)
    {
        if (resources == null || !resources.Any())
            return Result.Failure<RoadmapItem>(RoadmapErrors.ResourcesRequired);
            
        // Create the item with the resources attached
        var item = new RoadmapItem(title, resources, order);
        
        // Setting RoadmapItemId happens in the constructor now
        
        return item;
    }

    internal Result UpdateDetails(string title, List<ResourceLink> resources, int order)
    {
        if (resources == null || !resources.Any())
        {
            return Result.Failure(RoadmapErrors.ResourcesRequired);
        }
        
        Title = title.Trim();
       
        _resourceLinks.Clear();
        
        foreach (var resource in resources)
        {
            resource.SetRoadmapItem(Id);
            _resourceLinks.Add(resource);
        }

        return Result.Success();
    }
}