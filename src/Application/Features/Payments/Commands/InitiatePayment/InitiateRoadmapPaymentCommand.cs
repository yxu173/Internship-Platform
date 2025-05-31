using Application.Abstractions.Messaging;
using MediatR;
using Application.Common.Interfaces; 

namespace Application.Features.Payments.Commands.InitiatePayment;

public record InitiateRoadmapPaymentCommand(
    Guid StudentId,
    Guid RoadmapId
) : ICommand<PaymentInitiationResponse>; 