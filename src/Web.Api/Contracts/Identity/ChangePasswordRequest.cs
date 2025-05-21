namespace Web.Api.Contracts.Identity;

public sealed record ChangePasswordRequest(string OldPassword, string NewPassword);