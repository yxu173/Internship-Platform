using Application.Features.Skills.CreateSkill;
using Application.Features.Skills.DeleteSkill;
using Application.Features.Skills.GetAllSkills;
using Application.Features.Skills.UpdateSkill;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Skill;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;


public class SkillController : BaseController
{
    [HttpPost]
    public async Task<IResult> CreateSkill([FromBody] CreateSkillRequest request)
    {
        var command = new CreateSkillCommand(request.Name);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet]
    public async Task<IResult> GetSkills()
    {
        var query = new GetAllSkillsQuery();
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

     [HttpPut("{id}")]
    public async Task<IResult> UpdateSkill([FromRoute] Guid id)
    {
        var command = new UpdateSkillCommand(id);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteSkill([FromRoute] Guid id)
    {
        var command = new DeleteSkillCommand(id);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

}