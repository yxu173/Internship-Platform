using Application.Features.CompanyProfile.Commands.CreateCompanyProfile;
using Application.Features.CompanyProfile.Commands.UpdateCompanyAbout;
using Application.Features.CompanyProfile.Commands.UpdateCompanyBasicInfo;
using Application.Features.CompanyProfile.Commands.UpdateCompanyLogo;
using Application.Features.CompanyProfile.Queries.GetCompanyAbout;
using Application.Features.CompanyProfile.Queries.GetCompanyBasicInfo;
using Application.Features.CompanyProfile.Queries.GetCompanyLogo;
using Application.Features.CompanyProfile.Queries.GetCompanyProfile;
using Application.Features.CompanyProfile.Queries.GetCompleteCompanyProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.CompanyProfile;
using Web.Api.Contracts.Profile;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[Authorize]
public class CompanyController : BaseController
{
    [HttpGet("profiles/{id}")]
    public async Task<IResult> GetCompleteCompanyProfile(Guid id)
    {
        var query = new GetCompleteCompanyProfileQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }


    [HttpPost("profiles")]
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


    [HttpGet("profiles")]
    public async Task<IResult> GetCompanyProfile()
    {
        var query = new GetCompanyProfileQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    /*
    [HttpGet("profiles/{id:guid}")]
    public async Task<IResult> GetCompanyProfileById(Guid id)
    {
        var query = new GetCompanyProfileQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }*/

    [HttpPatch("profiles/me/basic")]
    public async Task<IResult> UpdateBasicInfo([FromBody] UpdateCompanyBasicInfoRequest request)
    {
        var command = new UpdateCompanyBasicInfoCommand(
            UserId,
            request.Name,
            request.Industry,
            request.Description,
            request.WebsiteUrl,
            request.CompanySize);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/{id:guid}/basic")]
    public async Task<IResult> GetCompanyBasicInfo([FromRoute] Guid id)
    {
        var query = new GetCompanyBasicInfoQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/{id:guid}/about")]
    public async Task<IResult> GetCompanyAbout([FromRoute] Guid id)
    {
        var query = new GetCompanyAboutQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/{id:guid}/about")]
    public async Task<IResult> UpdateCompanyAbout([FromRoute] Guid id, [FromBody] UpdateCompanyAboutRequest request)
    {
        var command = new UpdateCompanyAboutCommand(
            id,
            request.About,
            request.Mission,
            request.Vision
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/{id:guid}/logo")]
    public async Task<IResult> UpdateCompanyLogo([FromRoute] Guid id, [FromBody] string logoUrl)
    {
        var command = new UpdateCompanyLogoCommand(
            id,
            logoUrl
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/{id:guid}/logo")]
    public async Task<IResult> GetCompanyLogo([FromRoute] Guid id)
    {
        var query = new GetCompanyLogoQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}