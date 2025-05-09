using Application.Abstractions.Messaging;
using Domain.ValueObjects;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyAddress;

public sealed record UpdateCompanyAddressCommand(Guid UserId, string Governorate, string City) : ICommand<bool>;