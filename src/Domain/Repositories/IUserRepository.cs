using Domain.Aggregates.Users;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task CreateAsync(User? user, string password);
    Task UpdateAsync(User? user);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<User?>> GetUsersByRoleAsync(string role);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsUsernameUniqueAsync(string username);
    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<bool> ResetPasswordAsync(User user, string token, string newPassword);
    Task<User?> GetByIdWithStudentProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}