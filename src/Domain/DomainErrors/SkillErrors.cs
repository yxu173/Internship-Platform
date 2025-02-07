using SharedKernel;

namespace Domain.DomainErrors
{
    public sealed class SkillErrors
    {
        public static Error AlreadyExists => Error.Conflict(
            "Skill.AlreadyExists",
            "Skill already exists");

        public static Error NotFound => Error.NotFound(
            "Skill.NotFound",
            "Skill not found");
        
        public static Error InvalidName => Error.BadRequest(
            "Skill.InvalidName",
            "Skill name is invalid");

        // make one for if skills table is empty

        public static Error EmptySkills => Error.NotFound(
            "Skill.Empty",
            "No skills found in the system");

        // make one for if deleting a skill fails

        public static Error DeleteFailed => Error.Problem(
            "Skill.DeleteFailed",
            "Failed to delete skill");
    }
}