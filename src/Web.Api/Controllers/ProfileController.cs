using Application.Features.Profiles.StudentProfile;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Profile;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

public class ProfileController : BaseController
{
    [HttpPost("student")]
    public async Task<IResult> CreateStudentProfile([FromBody] CreateStudentProfileRequest request)
    {
        var yearResult = Year.Create(request.GraduationYear);
        var phoneResult = PhoneNumber.Create(request.PhoneNumber);
        
        if (yearResult.IsFailure)
            return Results.BadRequest(yearResult.Error);
        if (phoneResult.IsFailure)
            return Results.BadRequest(phoneResult.Error);

        var command = new CreateStudentProfileCommand
        {
            UserId = request.UserId,
            FullName = request.FullName,
            University = Enum.Parse<EgyptianUniversity>(request.University),
            Faculty = request.Faculty,
            GraduationYear = yearResult.Value,
            Age = request.Age,
            Gender = Enum.Parse<Gender>(request.Gender),
            PhoneNumber = phoneResult.Value
        };

        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}