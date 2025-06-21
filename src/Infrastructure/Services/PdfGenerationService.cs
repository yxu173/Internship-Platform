using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Services;
using IronPdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Services;

public class PdfGenerationService : IPdfGenerationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PdfGenerationService> _logger;
    private readonly string _storagePath;

    public PdfGenerationService(IConfiguration configuration, ILogger<PdfGenerationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _storagePath = _configuration["ResumeStorage:Path"] ?? "storage/resumes";
        
        // Ensure the storage directory exists
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
            _logger.LogInformation("Created resume storage directory: {StoragePath}", _storagePath);
        }
    }

    public async Task<Result<string>> GenerateResumePdfAsync(string markdownContent, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting PDF generation from markdown content");
            
            if (string.IsNullOrEmpty(markdownContent))
            {
                _logger.LogError("Markdown content is null or empty");
                return Result.Failure<string>(Error.Problem("PdfGeneration.Error", "Markdown content is null or empty"));
            }

            // Create a unique filename
            var fileName = $"resume_{Guid.NewGuid()}.pdf";
            var filePath = Path.Combine(_storagePath, fileName);

            _logger.LogInformation("Generating PDF at path: {FilePath}", filePath);

            // Create an instance of the ChromePdfRenderer
            var renderer = new ChromePdfRenderer();
            
            // Configure the renderer for better quality
            renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A4;
            renderer.RenderingOptions.MarginTop = 20;
            renderer.RenderingOptions.MarginBottom = 20;
            renderer.RenderingOptions.MarginLeft = 20;
            renderer.RenderingOptions.MarginRight = 20;
            renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print;

            // Render markdown string as PDF using IronPDF's built-in method
            var pdfDocument = renderer.RenderMarkdownStringAsPdf(markdownContent);
            
            // Save the PDF document to file
            pdfDocument.SaveAs(filePath);

            _logger.LogInformation("Successfully generated PDF resume at: {FilePath}", filePath);
            
            return Result.Success(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF from markdown");
            return Result.Failure<string>(Error.Problem("PdfGeneration.Error", $"Error generating PDF: {ex.Message}"));
        }
    }
} 