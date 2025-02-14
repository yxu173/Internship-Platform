using Application.Features.CompanyProfile.Commands.CreateCompanyProfile;
using Application.Features.CompanyProfile.Queries.GetCompanyProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Profile;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[Authorize]
public class CompanyController : BaseController
{
    [HttpPost("company")]
    public async Task<IResult> CreateCompanyProfile([FromBody] CreateCompanyProfileRequest request)
    {
        var command = new CreateCompanyProfileCommand(
            UserId,
            request.CompanyName,
            request.TaxId,
            request.Governorate,
            request.City,
            request.Street,
            request.Industry);

        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }


    [HttpGet("company")]
    public async Task<IResult> GetCompanyProfile()
    {
        var query = new GetCompanyProfileQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}