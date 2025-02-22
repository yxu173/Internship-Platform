﻿using Domain.Aggregates.Profiles;
using Domain.Common;
using Domain.Enums;

namespace Domain.Aggregates.Users;

public sealed class StudentSkill : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Guid SkillId { get; private set; }

    public StudentProfile Student { get; private set; }
    public Skill Skill { get; private set; }

    private StudentSkill() { }

    public StudentSkill(Guid studentId, Guid skillId)
    {
        Id = Guid.NewGuid(); 
        StudentId = studentId;
        SkillId = skillId;
    }
}