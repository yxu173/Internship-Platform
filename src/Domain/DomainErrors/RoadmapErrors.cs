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

    public static Error InvalidResourceType => Error.Validation(
        "RoadmapItem.InvalidResourceType",
        "The provided resource type for the roadmap item is invalid."
    );

    public static Error DuplicateEnrollment => Error.Conflict(
        "Enrollment.Duplicate",
        "Student is already enrolled in this roadmap."
    );

    public static Error PremiumEnrollmentRequiresPayment => Error.Validation(
        "Enrollment.PaymentRequired",
        "Enrollment in this premium roadmap requires payment."
    );

    public static Error EnrollmentNotFound => Error.NotFound(
        "Enrollment.NotFound",
        "Student enrollment not found for this roadmap."
    );
    
    public static Error QuizNotFound => Error.NotFound(
        "Quiz.NotFound",
        "Quiz not found for this section."
    );
    
    public static Error QuizAlreadyPassed => Error.Conflict(
        "Quiz.AlreadyPassed",
        "Quiz has already been passed for this section."
    );
    
    public static Error InvalidQuizAttempt => Error.Validation(
        "Quiz.InvalidAttempt",
        "This quiz attempt does not belong to this enrollment."
    );
    
    public static Error SectionNotAccessible => Error.Validation(
        "Section.NotAccessible",
        "This section is not accessible. Complete previous sections first."
    );
    
    public static Error NoQuestionsInQuiz => Error.Validation(
        "Quiz.NoQuestions",
        "Quiz must have at least one question."
    );
    
    public static Error QuestionNotFound => Error.NotFound(
        "Quiz.QuestionNotFound",
        "Question not found in this quiz."
    );
    
    public static Error InvalidQuizAnswer => Error.Validation(
        "Quiz.InvalidAnswer",
        "The provided answer is invalid for this question."
    );

    public static Error InvalidQuizParams => Error.Validation(
        "Quiz.InvalidParameters",
        "Quiz parameters are invalid. Passing score must be between 0 and 100."
    );

    public static Error OptionNotFound => Error.NotFound(
        "Quiz.OptionNotFound",
        "Option not found in this quiz."
    );
}