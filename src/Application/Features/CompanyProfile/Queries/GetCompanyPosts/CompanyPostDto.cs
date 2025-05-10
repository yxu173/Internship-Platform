
namespace Application.Features.CompanyProfile.Queries.GetCompanyPosts;

public class CompanyPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; }
    public int ApplicationCount { get; set; }
    public int BookmarkCount { get; set; }
    public string DaysRemaining { get; set; }
}
