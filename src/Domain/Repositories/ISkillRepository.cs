using Domain.Aggregates.Users;
using SharedKernel;

namespace Domain.Repositories;

public interface ISkillRepository
{
    Task<Skill> GetByIdAsync(Guid id);
    Task<IEnumerable<Skill>> GetAllAsync();
    Task<Result<bool>> CreateAsync(string name);
    Task<Result<Skill>> UpdateAsync(Guid id);

    Task<Result<bool>> DeleteAsync(Guid id);
}