using Domain.Common;
using SharedKernel;

namespace Domain.Aggregates.Profiles;

public sealed class StudentProject : BaseEntity
{
    public Guid StudentProfileId { get; private set; }
    public string ProjectName { get; private set; }
    public string Description { get; private set; }
    public string? ProjectUrl { get; private set; }
    public StudentProfile StudentProfile { get; private set; }

    private StudentProject()
    {
    }

    private StudentProject(
        Guid studentProfileId,
        string projectName,
        string description,
        string? projectUrl)
    {
        StudentProfileId = studentProfileId;
        ProjectName = projectName;
        Description = description;
        ProjectUrl = projectUrl;
    }

    public static Result<StudentProject> Create(
        Guid studentProfileId,
        string projectName,
        string description,
        string? projectUrl)
    {
        return Result.Success(new StudentProject(
            studentProfileId,
            projectName,
            description,
            projectUrl));
    }
    
    public Result Update(
        string projectName,
        string description,
        string? projectUrl)
    {
        ProjectName = projectName;
        Description = description;
        ProjectUrl = projectUrl;
        
        return Result.Success();
    }
}