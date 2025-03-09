using Domain.Common;
using Domain.Enums;

namespace Domain.ValueObjects;

public record ResourceLink : ValueObject
{
    public string Title { get; }
    public string Url { get; }
    public ResourceType Type { get; }

    private ResourceLink() { }
    public ResourceLink(string title, string url, string type)
    {
        var typeResult = Enum.Parse<ResourceType>(type);
        Title = title.Trim();
        Url = url.Trim();
        Type = typeResult;
    }

    //
    // public static ResourceLink CreateVideo(string title, string url)
    //     => new(title, url, ResourceType.Video);
    //
    // public static ResourceLink CreateBook(string title, string purchaseUrl)
    //     => new(title, purchaseUrl, ResourceType.Book);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Title;
        yield return Url;
        yield return Type;
    }
}