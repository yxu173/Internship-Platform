namespace Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Aggregates.Users;
    using Domain.DomainErrors;
    using Domain.Repositories;
    using Infrastructure.Database;
    using Microsoft.EntityFrameworkCore;
    using SharedKernel;

    public sealed class SkillRepository : ISkillRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SkillRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Skill> GetByIdAsync(Guid id)
        {
            return await _dbContext.Skills.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Result<Skill>> CreateAsync(string name)
        {
            var skill = Skill.Create(name);
            if (skill.IsFailure)
                return Result.Failure<Skill>(SkillErrors.AlreadyExists);
            await _dbContext.Skills.AddAsync(skill.Value);
            await _dbContext.SaveChangesAsync();
            return Result.Success(skill.Value);
        }

        public async Task<Result<Skill>> UpdateAsync(Guid id)
        {
            var skill = await GetByIdAsync(id);

            if (skill == null)
                return Result.Failure<Skill>(SkillErrors.NotFound);

            _dbContext.Skills.Update(skill);
            await _dbContext.SaveChangesAsync();

            return Result.Success(skill);
        }

        public async Task<IEnumerable<Skill>> GetAllAsync()
        {
            var result = await _dbContext.Skills.ToListAsync();
            if (result == null)
                return (IEnumerable<Skill>)Result.Failure<IEnumerable<Skill>>(SkillErrors.EmptySkills);

            return result;
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var skill = await GetByIdAsync(id);
            if (skill == null)
                return Result.Failure<bool>(SkillErrors.NotFound);

            _dbContext.Skills.Remove(skill);
            await _dbContext.SaveChangesAsync();

            return Result.Success(true);
        }
    }
}