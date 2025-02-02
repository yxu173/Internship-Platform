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
     EgyptianUniversity university
     , string faculty
     , Year graduationYear
     , int age
     , Gender gender
        , PhoneNumber phoneNumber);
    Task UpdateAsync(StudentProfile student);
}
