using Application.Abstractions.Messaging;
namespace Application.Features.Payments.Commands.ProcessCallback;

public record ProcessPaymobCallbackCommand(
    string Payload,
    string Signature
) : ICommand; 