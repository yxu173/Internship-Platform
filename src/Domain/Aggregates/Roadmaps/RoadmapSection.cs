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

    public Guid RoadmapId { get; private set; }
    public string Title { get; private set; }
    public int Order { get; private set; }

    public IReadOnlyList<RoadmapItem> Items => _items.AsReadOnly();

    public RoadmapSection(string title, int order, Guid roadmapId)
    {
        Title = title.Trim();
        Order = order;
        RoadmapId = roadmapId;
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

    internal void UpdateDetails(string title, int order)
    {
        Title = title?.Trim();
        Order = order;
    }

    public Result<Guid> AddItem(string title, string description, string type,
        List<ResourceLink> resources, int order)
    {
        if (_items.Any(i => i.Order == order))
        {
            return Result.Failure<Guid>(RoadmapErrors.DuplicateItemOrder);
        }

        var itemResult = RoadmapItem.Create(title, description, type, resources, order);
        if (itemResult.IsFailure)
        {
            return Result.Failure<Guid>(itemResult.Error);
        }

        _items.Add(itemResult.Value);

        return Result.Success(itemResult.Value.Id);
    }

    public Result UpdateItem(Guid itemId, string title, string description, string type,
        List<ResourceLink> resources, int order)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return Result.Failure(RoadmapErrors.ItemNotFound);
        }

        if (_items.Any(i => i.Id != itemId && i.Order == order))
        {
            return Result.Failure(RoadmapErrors.DuplicateItemOrder);
        }

        var updateResult = item.UpdateDetails(title, description, type, resources, order);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }


        return Result.Success();
    }

    public Result RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return Result.Success();
        }

        _items.Remove(item);
        return Result.Success();
    }
}