namespace Infrastructure.Resilience;

public sealed class ResilienceOptions
{
    public const string SectionName = "Resilience";

    public string? ExternalApiBaseUrl { get; set; }

    public int HttpTimeoutSeconds { get; set; } = 10;

    public int RetryCount { get; set; } = 3;

    public double RetryBackoffBaseSeconds { get; set; } = 0.5;

    public int CircuitBreakerFailuresAllowedBeforeBreaking { get; set; } = 5;

    public int CircuitBreakerBreakDurationSeconds { get; set; } = 30;
}
