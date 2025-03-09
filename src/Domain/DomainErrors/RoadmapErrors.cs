using SharedKernel;

namespace Domain.DomainErrors;

public static class RoadmapErrors
{
    public static Error RoadmapNotFound => Error.NotFound(
        "Roadmap.NotFound",
        "Roadmap not found"
    );

    public static Error PaidRoadmap => Error.Validation(
        "Roadmap.PaidRoadmap",
        "Roadmap is already paid");

    public static Error DuplicateProgress => Error.Conflict(
        "Roadmap.DuplicateProgress",
        "Item progress is already marked");

    public static Error PremiumRequiresPrice => Error.Validation(
        "Roadmap.PremiumRequiresPrice",
        "Premium roadmaps require a price");

    public static Error InvalidTechnology => Error.Validation(
        "Roadmap.InvalidTechnology",
        "Technology is required for the roadmap"
    );

    public static Error DuplicateSection => Error.Conflict(
        "Roadmap.DuplicateSection",
        "Section already exists in the roadmap"
    );

    public static Error DuplicateSectionOrder => Error.Conflict(
        "Roadmap.DuplicateSectionOrder",
        "Section order already exists in the roadmap"
    );

    public static Error DuplicateItemOrder => Error.Conflict(
        "Roadmap.DuplicateItemOrder",
        "Item order already exists in the roadmap"
    );

    public static Error SectionNotFound => Error.NotFound(
        "Roadmap.SectionNotFound",
        "Section not found in the roadmap"
    );

    public static Error ResourcesRequired => Error.Validation(
        "Roadmap.ResourcesRequired",
        "Resources are required for the roadmap item"
    );

    public static Error ItemNotFound => Error.NotFound(
        "Roadmap.ItemNotFound",
        "Item not found in the roadmap"
    );

    public static Error NoItemsProvided => Error.Validation(
        "Roadmap.NoItemsProvided",
        "No items provided to add to the roadmap section"
    );

    public static Error InvalidStudentId => Error.Validation(
        "Roadmap.InvalidStudentId",
        "Student id is required to mark progress"
    );

    public static Error InvalidRoadmapId => Error.Validation(
        "Roadmap.InvalidRoadmapId",
        "Roadmap id is required to mark progress"
    );

    public static Error InvalidOrDuplicateItem => Error.Validation(
        "Roadmap.InvalidOrDuplicateItem",
        "Item is invalid or already marked as completed"
    );
}