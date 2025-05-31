using Domain.Common;
using SharedKernel;

namespace Domain.ValueObjects;

public sealed record CompanyAbout : ValueObject
{
    public string? About { get; private set; }
    public string? Mission { get; private set; }
    public string? Vision { get; private set; }

    private CompanyAbout(string about, string mission, string vision)
        => (About, Mission, Vision) = (about, mission, vision);

    public Result Update(string about, string mission, string vision)
    {
        About = about;
        Mission = mission;
        Vision = vision;
        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return About;
        yield return Mission;
        yield return Vision;
    }
}