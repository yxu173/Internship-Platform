using FluentValidation;

namespace Application.Features.CompanyProfile.Commands.CreateCompanyProfile;

public sealed class CreateCompanyProfileCommandValidator : AbstractValidator<CreateCompanyProfileCommand>
{
    public CreateCompanyProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required")
            .MinimumLength(2)
            .WithMessage("Company name must be at least 2 characters")
            .MaximumLength(100)
            .WithMessage("Company name cannot exceed 100 characters");

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .WithMessage("Tax ID is required")
            .Matches(@"^3\d{13}$")
            .WithMessage("Tax ID must be 14 digits long and start with 3");  

        RuleFor(x => x.Governorate)
            .NotEmpty()
            .WithMessage("Governorate is required")
            .Must(g => Enum.TryParse<Domain.Enums.Governorate>(g, out _))
            .WithMessage("Invalid governorate value");
        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MinimumLength(2)
            .WithMessage("City must be at least 2 characters")
            .MaximumLength(50)
            .WithMessage("City cannot exceed 50 characters");
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required");

        RuleFor(x => x.Industry)
            .NotEmpty()
            .WithMessage("Industry is required")
            .MinimumLength(2)
            .WithMessage("Industry must be at least 2 characters")
            .MaximumLength(50)
            .WithMessage("Industry cannot exceed 50 characters");
    }
}