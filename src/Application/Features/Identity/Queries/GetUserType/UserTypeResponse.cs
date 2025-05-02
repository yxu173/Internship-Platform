namespace Application.Features.Identity.Queries.GetUserType;

public sealed record UserTypeResponse(
    string UserType,
    Guid? ProfileId
); 