using Domain.Aggregates.Profiles;
using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class Roadmap : BaseAuditableEntity
{
    private Roadmap()
    {
    }

    private readonly List<RoadmapSection> _sections = new();
    private readonly List<Enrollment> _enrollments = new();
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsPremium { get; private set; }
    public decimal? Price { get; private set; }
    public Guid CompanyId { get; private set; }
    public CompanyProfile? Company { get; private set; }
    public string Technology { get; private set; }
    public IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyList<RoadmapSection> Sections => _sections.AsReadOnly();

    private Roadmap(
        string title,
        string description,
        string technology,
        bool isPremium,
        decimal? price,
        Guid companyId)
    {
        Title = title.Trim();
        Description = description.Trim();
        Technology = technology;
        IsPremium = isPremium;
        Price = price;
        CompanyId = companyId;
    }

    public static Result<Roadmap> Create(
        string title,
        string description,
        string technology,
        bool isPremium,
        decimal? price,
        Guid companyId)
    {
        if (isPremium && !price.HasValue)
            return Result.Failure<Roadmap>(RoadmapErrors.PremiumRequiresPrice);

        if (string.IsNullOrWhiteSpace(technology))
            return Result.Failure<Roadmap>(RoadmapErrors.InvalidTechnology);

        return new Roadmap(
            title,
            description,
            technology,
            isPremium,
            price,
            companyId
        );
    }

    public Result AddSection(string title, int order, List<RoadmapItem> items)
    {
        if (_sections.Any(s => s.Order == order))
            return Result.Failure(RoadmapErrors.DuplicateSectionOrder);

        var section = new RoadmapSection(title, order);
        var orderedItems = items.OrderBy(i => i.Order).ToList();

        if (orderedItems.Select(i => i.Order).Distinct().Count() != orderedItems.Count)
            return Result.Failure(RoadmapErrors.DuplicateItemOrder);

        section.AddItems(orderedItems);
        _sections.Add(section);
        ModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }
}