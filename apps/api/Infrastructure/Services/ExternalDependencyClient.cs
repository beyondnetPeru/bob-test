using Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public sealed class ExternalDependencyClient(HttpClient httpClient, ILogger<ExternalDependencyClient> logger) : IExternalDependencyClient
{
    public async Task<bool> IsAvailableAsync(string relativePath = "/health", CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync(relativePath, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "External dependency availability check failed for path {Path}", relativePath);
            return false;
        }
    }
}
