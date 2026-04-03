namespace Application.Abstractions.Services;

public interface IExternalDependencyClient
{
    Task<bool> IsAvailableAsync(string relativePath = "/health", CancellationToken cancellationToken = default);
}
