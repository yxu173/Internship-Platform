using Domain.Common;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class Roadmap : BaseAuditableEntity
{
    private readonly List<RoadmapSection> _sections = new();
    private readonly List<Enrollment> _enrollments = new();

    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsPremium { get; private set; }
    public decimal? Price { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Technology { get; private set; }

    public IReadOnlyList<RoadmapSection> Sections => _sections.AsReadOnly();
    public IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();

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
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Roadmap> CreatePremium(
        string title,
        string description,
        string technology,
        decimal price,
        Guid companyId)
    {
        return new Roadmap(
            title: title,
            description: description,
            technology: technology,
            isPremium: true,
            price: price,
            companyId: companyId
        );
    }

    public Result AddSection(
        string title,
        int order,
        bool isPremiumSection,
        List<RoadmapItem> items)
    {
        var section = new RoadmapSection(
            title: title.Trim(),
            order: order,
            isPremium: isPremiumSection && this.IsPremium
        );

        section.AddItems(items);
        _sections.Add(section);
        return Result.Success();
    }
}