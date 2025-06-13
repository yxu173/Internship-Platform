using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.ResumeGeneration;

public record GenerateResumeCommand(Guid UserId, Guid InternshipId) : ICommand<GenerateResumeResponse>;
public class GenerateResumeResponse
{
    public Guid ResumeId { get; set; }
    public string FilePath { get; set; }
    public string DownloadUrl { get; set; }
    
    public GenerateResumeResponse(Guid resumeId, string filePath, string downloadUrl)
    {
        ResumeId = resumeId;
        FilePath = filePath;
        DownloadUrl = downloadUrl;
    }
}
