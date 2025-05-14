namespace Application.Abstractions.Services;

public interface IGeminiClient
{
    Task<T> InvokeFunctionAsync<T>(string functionName, object functionParameters, byte[] templateFile = null,
        CancellationToken cancellationToken = default);
}