using System;

namespace Application.Features.ResumeGeneration;

public class GenerateResumeRequest
{
    public Guid InternshipId { get; set; }
    public string TemplateName { get; set; }
}
