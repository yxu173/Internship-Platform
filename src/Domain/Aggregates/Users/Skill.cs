using Domain.Common;

namespace Domain.Aggregates.Users;

public class Skill : BaseEntity
{
    public string Name { get; private set; }
    public ICollection<StudentSkill> StudentSkills { get; private set; } = new List<StudentSkill>();

    private Skill()
    {
    }

    public Skill(string name)
    {
        Name = name;
    }
}