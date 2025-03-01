using SharedKernel;

namespace Domain.DomainErrors;

public static class RoadmapErrors
{
    public static Error PaidRoadmap => Error.Validation(
        "Roadmap.PaidRoadmap",
        "Roadmap is already paid");
}