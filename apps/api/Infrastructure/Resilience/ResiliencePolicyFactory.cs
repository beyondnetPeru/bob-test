using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Infrastructure.Resilience;

public static class ResiliencePolicyFactory
{
    public static IAsyncPolicy<HttpResponseMessage> BuildRetryPolicy(
        ResilienceOptions options,
        ILogger logger,
        string dependency)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: options.RetryCount,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(options.RetryBackoffBaseSeconds, attempt)),
                onRetry: (outcome, delay, attempt, _) =>
                {
                    var code = outcome.Result?.StatusCode;
                    logger.LogWarning(
                        "Retry {Attempt}/{MaxRetries} for dependency {Dependency}. Delay={DelayMs}ms StatusCode={StatusCode} Error={Error}",
                        attempt,
                        options.RetryCount,
                        dependency,
                        delay.TotalMilliseconds,
                        code,
                        outcome.Exception?.Message
                    );
                }
            );
    }

    public static IAsyncPolicy<HttpResponseMessage> BuildCircuitBreakerPolicy(
        ResilienceOptions options,
        ILogger logger,
        string dependency)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: options.CircuitBreakerFailuresAllowedBeforeBreaking,
                durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreakerBreakDurationSeconds),
                onBreak: (outcome, breakDelay) =>
                {
                    var code = outcome.Result?.StatusCode;
                    logger.LogError(
                        "Circuit opened for dependency {Dependency}. BreakDurationMs={BreakDurationMs} StatusCode={StatusCode} Error={Error}",
                        dependency,
                        breakDelay.TotalMilliseconds,
                        code,
                        outcome.Exception?.Message
                    );
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit reset for dependency {Dependency}", dependency);
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit half-open for dependency {Dependency}", dependency);
                }
            );
    }
}
