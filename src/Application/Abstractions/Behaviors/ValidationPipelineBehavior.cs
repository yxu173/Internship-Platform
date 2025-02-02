using FluentValidation;
using FluentValidation.Results;
using MediatR;
using SharedKernel;

namespace Application.Abstractions.Behaviors;


public sealed class ValidationPipelineBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            return CreateValidationResult<TResponse>(failures);

        return await next();
    }

    private static TResponse CreateValidationResult<TResponse>(List<ValidationFailure> failures)
        where TResponse : Result
    {
        var errors = failures
            .Select(f => Error.Validation(f.ErrorCode, f.ErrorMessage))
            .ToArray();

        var validationError = new ValidationError(errors);

        if (typeof(TResponse) == typeof(Result))
            return (Result.Failure(validationError) as TResponse)!;

        var result = typeof(Result<>)
            .MakeGenericType(typeof(TResponse).GenericTypeArguments[0])
            .GetMethod(nameof(Result.Failure))!
            .Invoke(null, new object?[] { validationError })!;

        return (TResponse)result;
    }
}