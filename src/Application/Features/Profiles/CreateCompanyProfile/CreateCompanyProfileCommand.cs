using Application.Abstractions.Messaging;

namespace Application.Features.Profiles.CreateCompanyProfile;

public sealed record CreateCompanyProfileCommand(Guid UserId,
 string CompanyName,
  string TaxId,
   string Governorate,
    string Industry) : ICommand<bool>;