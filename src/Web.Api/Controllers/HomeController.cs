using Application.Features.Home.Queries.GetHomePageData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[Authorize]
public class HomeController : BaseController
{
    [HttpGet("GetHomePageData")]
    public async Task<IResult> GetHomePageData()
    {
        var query = new GetHomePageDataQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
