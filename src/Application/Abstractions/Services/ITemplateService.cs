using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions.Services;

public interface ITemplateService
{
    Task<byte[]> GetResumeTemplateAsync(string templateName, CancellationToken cancellationToken = default);

    Task<string[]> GetAvailableResumeTemplatesAsync(CancellationToken cancellationToken = default);
}