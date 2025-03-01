using Domain.Common;

namespace Domain.Aggregates.Roadmaps;

// Domain/Aggregates/Roadmaps/RoadmapSection.cs
public sealed class RoadmapSection : BaseEntity
{
    private readonly List<RoadmapItem> _items = new();
    
    public string Title { get; }
    public int Order { get; }
    public bool IsPremium { get; }

    public IReadOnlyList<RoadmapItem> Items => _items.AsReadOnly();

    public RoadmapSection(string title, int order, bool isPremium)
    {
        Title = title.Trim();
        Order = order;
        IsPremium = isPremium;
    }

    public void AddItems(List<RoadmapItem> items)
    {
        _items.AddRange(items.OrderBy(i => i.Order));
    }
}