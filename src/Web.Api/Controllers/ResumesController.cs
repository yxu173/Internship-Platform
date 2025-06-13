using System;
using System.Threading.Tasks;
using Application.Abstractions.Services;
using Application.Features.ResumeGeneration;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Web.Api.Controllers;

public class ResumesController : BaseController
{
    private readonly ILogger<ResumesController> _logger;

    public ResumesController(ILogger<ResumesController> logger)
    {
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<ActionResult> GenerateResume([FromBody] GenerateResumeRequest request)
    {
        try
        {
            _logger.LogInformation("Starting resume generation for user {UserId} and internship {InternshipId}", UserId, request.InternshipId);
            
            var command = new GenerateResumeCommand(UserId, request.InternshipId);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogError("Resume generation failed: {Error}", result.Error.ToString());
                return BadRequest(new { error = result.Error.ToString() });
            }

            _logger.LogInformation("Resume generation completed successfully. Resume ID: {ResumeId}", result.Value.ResumeId);
            
            return Created(result.Value.DownloadUrl, new
            {
                resumeId = result.Value.ResumeId,
                downloadUrl = result.Value.DownloadUrl
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation during resume generation");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during resume generation");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("download/{resumeId:guid}")]
    public async Task<IActionResult> DownloadResume(Guid resumeId)
    {
        try
        {
            _logger.LogInformation("Downloading resume {ResumeId} for user {UserId}", resumeId, UserId);
            
            var resumeRepository = HttpContext.RequestServices.GetRequiredService<IGeneratedResumeRepository>();
            var resume = await resumeRepository.GetByIdAsync(resumeId);
            
            if (resume == null)
            {
                _logger.LogWarning("Resume {ResumeId} not found", resumeId);
                return NotFound(new { error = "Resume not found" });
            }
            
            // Check if the user owns this resume
            if (resume.StudentId != UserId)
            {
                _logger.LogWarning("User {UserId} attempted to access resume {ResumeId} owned by {OwnerId}", UserId, resumeId, resume.StudentId);
                return Forbid();
            }
            
            if (!System.IO.File.Exists(resume.FilePath))
            {
                _logger.LogWarning("Resume file not found at path: {FilePath}", resume.FilePath);
                return NotFound(new { error = "Resume file not found" });
            }
            
            var fileName = System.IO.Path.GetFileName(resume.FilePath);
            
            // Since we're now generating PDF files, always return PDF content type
            var contentType = "application/pdf";
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(resume.FilePath);
            
            _logger.LogInformation("Successfully downloaded resume {ResumeId} for user {UserId}", resumeId, UserId);
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading resume {ResumeId} for user {UserId}", resumeId, UserId);
            return BadRequest(new { error = ex.Message });
        }
    }
}