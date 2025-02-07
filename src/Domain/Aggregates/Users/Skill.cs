using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.Aggregates.Users;

public sealed class Skill : BaseAuditableEntity
{
    private readonly List<StudentSkill> _studentSkills = new();

    public string Name { get; private set; }
    public IReadOnlyCollection<StudentSkill> StudentSkills => _studentSkills.AsReadOnly();

    private Skill() { }

    private Skill(string name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Skill> Create(string name)
    {

        var result = Result.Success(new Skill(name.Trim()));
        if (result.IsFailure)
            return Result.Failure<Skill>(SkillErrors.AlreadyExists);

        return result;
    }

    public void Update(string name)
    {
        Name = name;
        ModifiedAt = DateTime.UtcNow;
    }
}