using SharedKernel;

namespace Application.Abstractions.Services;

public interface IPdfGenerationService
{
    Task<Result<string>> GenerateResumePdfAsync(string markdownContent, CancellationToken cancellationToken = default);
} 