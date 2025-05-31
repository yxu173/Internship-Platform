namespace Application.Features.Identity.Login;

public sealed record LoginResponse(
    string Token,
    string UserType,
    Guid? ProfileId);