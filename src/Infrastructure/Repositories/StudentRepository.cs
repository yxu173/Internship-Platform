using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Domain.Enums;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result> CreateAsync(Guid userId,
     string fullName,
      EgyptianUniversity university,
       string faculty,
        Year graduationYear,
        int age,
         Gender gender,
            PhoneNumber phoneNumber
          )
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return Result.Failure<bool>(UserErrors.UserNotFound);
        }
        var student = user.CreateStudentProfile(
            fullName,
            university,
            faculty,
            graduationYear,
            age,
            gender,
            phoneNumber.Value);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<StudentProfile> GetByIdAsync(Guid id)
    {
        return await _context.StudentProfiles.FirstOrDefaultAsync(s => s.Id == id);
    }


    public async Task UpdateAsync(StudentProfile student)
    {
        _context.StudentProfiles.Update(student);
        await _context.SaveChangesAsync();
    }
}