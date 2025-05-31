using System;
using System.Threading.Tasks;
using Application.Abstractions.Services;
using Application.Features.ResumeGeneration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Web.Api.Controllers;

public class ResumesController : BaseController
{

    [HttpPost("generate")]
    public async Task<ActionResult> GenerateResume([FromBody] GenerateResumeRequest request)
    {
        try
        {
            var command = new GenerateResumeCommand(UserId, request.InternshipId, request.TemplateName);
            var result = await _mediator.Send(command);

            return Created(result.Value.DownloadUrl, new
            {
                resumeId = result.Value.ResumeId,
                downloadUrl = result.Value.DownloadUrl
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetResume(Guid id)
    {
       //TODO: make it 
        return StatusCode(501, "Not yet implemented");
    }
    
  
    [HttpGet("templates")]
    public async Task<ActionResult<IEnumerable<string>>> GetTemplates()
    {
        var templateService = HttpContext.RequestServices.GetRequiredService<ITemplateService>();
        var templates = await templateService.GetAvailableResumeTemplatesAsync();
        return Ok(templates);
    }
}