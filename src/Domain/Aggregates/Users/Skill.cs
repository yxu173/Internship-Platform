using Domain.Common;
using SharedKernel;

namespace Domain.Aggregates.Users;

public sealed class Skill : BaseEntity
{
    private readonly List<StudentSkill> _studentSkills = new();

    public string Name { get; private set; }
    public IReadOnlyCollection<StudentSkill> StudentSkills => _studentSkills.AsReadOnly();

    private Skill() { }

    private Skill(string name)
    {
    }

    public static Result<Skill> Create(string name)
    {
        try
        {
            return Result.Success(new Skill(name.Trim()));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Skill>(Error.Validation("Skill.Create", ex.Message));
        }
    }
}