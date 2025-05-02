using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Api.Infrastructure;
using Application.Features.Payments.Commands.InitiatePayment; // Assuming command location
using Application.Features.Payments.Commands.ProcessCallback; // Assuming command location
using Microsoft.AspNetCore.Authorization; // Likely needed for initiation
using Microsoft.Extensions.Logging; // For logging callback details
using System.IO; // To read callback body
using Microsoft.AspNetCore.Http; // For StatusCodes
using System.Linq; // For FirstOrDefault()
using System;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Mime;
using Web.Api.Extensions;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/payments")]
[Produces(MediaTypeNames.Application.Json)]
public class PaymentController : BaseController
{
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ISender mediator, ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

    [HttpPost("initiate-roadmap-purchase/{roadmapId:guid}")]
    //   [Authorize(Roles = "Student")] 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> InitiateRoadmapPayment([FromRoute] Guid roadmapId)
    {
        var command = new InitiateRoadmapPaymentCommand(UserId, roadmapId);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("test-cards")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IResult GetTestCards()
    {
        var testCards = new[]
        {
            new
            {
                Type = "Visa",
                Number = "4111111111111111",
                Month = "12",
                Year = "25",
                Cvv = "123",
                Name = "Test Account",
                Description = "Successful payment (3DS)"
            }
        };

        return Results.Ok(testCards);
    }

    [HttpPost("paymob-callback")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PaymobCallback()
    {
        string payload;
        using (var reader = new StreamReader(Request.Body))
        {
            payload = await reader.ReadToEndAsync();
        }

        Request.Query.TryGetValue("hmac", out var signatureValues);
        var signature = signatureValues.FirstOrDefault();

        _logger.LogInformation("Received Paymob callback. Signature: {Signature}, Payload: {Payload}",
            signature ?? "N/A", payload);

        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload from Paymob callback.");
            return BadRequest("Empty payload.");
        }

        var command = new ProcessPaymobCallbackCommand(payload, signature ?? string.Empty);
        var result = await _mediator.Send(command);

        return result.Match<IActionResult>(
            () => Ok(),
            problem => { return StatusCode(StatusCodes.Status400BadRequest, problem); }
        );
    }
}