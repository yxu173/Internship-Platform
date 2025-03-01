using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class RoadmapSection : BaseEntity
{
    private RoadmapSection()
    {
    }

    private readonly List<RoadmapItem> _items = new();

    public string Title { get; private set; }
    public int Order { get; private set; }

    public IReadOnlyList<RoadmapItem> Items => _items.AsReadOnly();

    public RoadmapSection(string title, int order)
    {
        Title = title.Trim();
        Order = order;
    }

    public Result AddItems(List<RoadmapItem> items)
    {
        if (items == null || !items.Any())
            return Result.Failure(RoadmapErrors.NoItemsProvided);
        if (items.Any(i => _items.Any(existing => existing.Order == i.Order)))
            return Result.Failure(RoadmapErrors.DuplicateItemOrder);
        _items.AddRange(items.OrderBy(i => i.Order));
        return Result.Success();
    }
}