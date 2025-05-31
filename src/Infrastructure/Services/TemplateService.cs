using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class TemplateService : ITemplateService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TemplateService> _logger;
    private readonly string _templatesPath;

    public TemplateService(IConfiguration configuration, ILogger<TemplateService> logger = null)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Get templates path from configuration or use default
        var storagePath = _configuration["StoragePath"] ?? "storage";
        _templatesPath = Path.Combine(storagePath, "templates", "resumes");
        
        // Ensure directory exists
        Directory.CreateDirectory(_templatesPath);
    }

    public async Task<byte[]> GetResumeTemplateAsync(string templateName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(templateName))
        {
            _logger?.LogWarning("Template name is null or empty");
            return null;
        }
        
        if (!templateName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            templateName += ".pdf";
        }
        
        var templatePath = Path.Combine(_templatesPath, templateName);
        
        if (!File.Exists(templatePath))
        {
            _logger?.LogWarning("Template file not found: {TemplatePath}", templatePath);
            return null;
        }
        
        try
        {
            return await File.ReadAllBytesAsync(templatePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error reading template file: {TemplatePath}", templatePath);
            return null;
        }
    }

    public Task<string[]> GetAvailableResumeTemplatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var templates = Directory.GetFiles(_templatesPath, "*.pdf")
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
                
            return Task.FromResult(templates);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting available templates from {TemplatesPath}", _templatesPath);
            return Task.FromResult(Array.Empty<string>());
        }
    }
}
