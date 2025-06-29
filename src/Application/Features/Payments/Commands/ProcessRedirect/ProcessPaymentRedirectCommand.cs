using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Payments.Commands.ProcessRedirect;

public sealed record ProcessPaymentRedirectCommand(
    string OrderId,
    string TransactionId) : ICommand<Guid>; 