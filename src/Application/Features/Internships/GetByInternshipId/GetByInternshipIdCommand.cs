using Application.Abstractions.Messaging;
using Application.Features.Internships.GetInternshipsByCompanyId;

namespace Application.Features.Internships.GetByInternshipId;

public sealed record GetByInternshipIdCommand(Guid InternshipId) : ICommand<InternshipResponse>;