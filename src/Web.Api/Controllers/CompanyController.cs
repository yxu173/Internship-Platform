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
    [HttpGet("profiles")]
    public async Task<IResult> GetCompleteCompanyProfile()
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


    // [HttpGet("profiles")]
    // public async Task<IResult> GetCompanyProfile()
    // {
    //     var query = new GetCompanyProfileQuery(UserId);
    //     var result = await _mediator.Send(query);
    //     return result.Match(Results.Ok, CustomResults.Problem);
    // }

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

    [HttpGet("profiles/basic")]
    public async Task<IResult> GetCompanyBasicInfo()
    {
        var query = new GetCompanyBasicInfoQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/about")]
    public async Task<IResult> GetCompanyAbout()
    {
        var query = new GetCompanyAboutQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/about")]
    public async Task<IResult> UpdateCompanyAbout( [FromBody] UpdateCompanyAboutRequest request)
    {
        var command = new UpdateCompanyAboutCommand(
            UserId,
            request.About,
            request.Mission,
            request.Vision
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/logo")]
    public async Task<IResult> UpdateCompanyLogo( [FromBody] string logoUrl)
    {
        var command = new UpdateCompanyLogoCommand(
            UserId,
            logoUrl
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/logo")]
    public async Task<IResult> GetCompanyLogo()
    {
        var query = new GetCompanyLogoQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}