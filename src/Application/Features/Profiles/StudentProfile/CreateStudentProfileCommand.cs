using Application.Abstractions.Messaging;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Features.Profiles.StudentProfile;


public sealed class CreateStudentProfileCommand : ICommand<bool>
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public EgyptianUniversity University { get; set; }
    public string Faculty { get; set; }
    public Year GraduationYear { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
}