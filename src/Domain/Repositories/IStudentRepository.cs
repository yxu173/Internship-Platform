using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using Domain.Enums;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Repositories;
public interface IStudentRepository
{
    Task<StudentProfile> GetByIdAsync(Guid id);
    Task<Result> CreateAsync(Guid userId,
     string fullName,
      string university,
       string faculty,
        int graduationYear,
        int enrollmentYear,
        int age,
        string gender,
        string phoneNumber);
    Task UpdateAsync(StudentProfile student);
    Task<StudentProfile?> GetByUserIdAsync(Guid userId);
}
