using Domain.Common;
using Domain.Enums;

namespace Domain.ValueObjects;

public record ResourceLink : ValueObject
{
    public string Title { get; }
    public string Url { get; }
    public ResourceType Type { get; }

    public ResourceLink(string title, string url, ResourceType type)
    {
        Title = title.Trim();
        Url = url.Trim();
        Type = type;
    }

    public static ResourceLink CreateVideo(string title, string url) 
        => new(title, url, ResourceType.Video);

    public static ResourceLink CreateBook(string title, string purchaseUrl) 
        => new(title, purchaseUrl, ResourceType.Book);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Title;
        yield return Url;
        yield return Type;
    }
}