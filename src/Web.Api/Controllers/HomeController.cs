using Application.Features.Home.Queries.GetHomePageData;
using Application.Features.Home.Queries.Search;
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
    
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IResult> Search(
        [FromQuery] string term,
        [FromQuery] SearchType type = SearchType.All,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        Guid? userId = HttpContext.User.Identity?.IsAuthenticated == true ? UserId : null;
        
        var query = new SearchQuery(term, type, userId, page, pageSize);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
