using Domain.Common;
using SharedKernel;
using System;

namespace Domain.Aggregates.Resumes;

public sealed class GeneratedResume : BaseAuditableEntity
{
    private GeneratedResume()
    {
    }

    public Guid StudentId { get; private set; }
    public Guid InternshipId { get; private set; }
    public string FilePath { get; private set; }
    public bool IsLatest { get; private set; }

    private GeneratedResume(
        Guid studentId,
        Guid internshipId,
        string filePath)
    {
        StudentId = studentId;
        InternshipId = internshipId;
        FilePath = filePath;
        IsLatest = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<GeneratedResume> Create(
        Guid studentId,
        Guid internshipId,
        string filePath)
    {
        return Result.Success(new GeneratedResume(
            studentId,
            internshipId,
            filePath));
    }

    public void MarkAsObsolete()
    {
        IsLatest = false;
    }
}
