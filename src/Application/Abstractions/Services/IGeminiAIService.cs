using Application.Features.ResumeGeneration;
using SharedKernel;

namespace Application.Abstractions.Services;

public interface IGeminiAIService
{
    Task<Result<string>> GenerateResumeMarkdownAsync(ResumeDto resumeData, CancellationToken cancellationToken = default);
} 