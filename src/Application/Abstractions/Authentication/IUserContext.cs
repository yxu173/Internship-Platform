namespace Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
    Guid? StudentId { get; }
    Guid? CompanyId { get; }
}