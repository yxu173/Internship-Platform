using Application.Abstractions.Messaging;

namespace Application.Features.Identity.Queries.GetUserType;

public sealed record GetUserTypeQuery(Guid UserId) : IQuery<UserTypeResponse>; 