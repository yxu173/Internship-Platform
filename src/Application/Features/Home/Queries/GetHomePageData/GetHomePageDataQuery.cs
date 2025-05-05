using Application.Abstractions.Messaging;

namespace Application.Features.Home.Queries.GetHomePageData;

public sealed record GetHomePageDataQuery(Guid UserId) : IQuery<HomePageDto>;
