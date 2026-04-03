using System.Net;
using Application.Abstractions.Services;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Resilience;

public static class DependencyInjection
{
    public static IServiceCollection AddApiResilience(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration
            .GetSection(ResilienceOptions.SectionName)
            .Get<ResilienceOptions>() ?? new ResilienceOptions();

        services
            .AddHttpClient<IExternalDependencyClient, ExternalDependencyClient>("external-api", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(options.HttpTimeoutSeconds);

                if (!string.IsNullOrWhiteSpace(options.ExternalApiBaseUrl))
                {
                    client.BaseAddress = new Uri(options.ExternalApiBaseUrl);
                }
            })
            .AddPolicyHandler((sp, request) =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Polly.Retry");
                var dependency = request.RequestUri?.Host ?? "external-api";
                return ResiliencePolicyFactory.BuildRetryPolicy(options, logger, dependency);
            })
            .AddPolicyHandler((sp, request) =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Polly.CircuitBreaker");
                var dependency = request.RequestUri?.Host ?? "external-api";
                return ResiliencePolicyFactory.BuildCircuitBreakerPolicy(options, logger, dependency);
            });

        return services;
    }
}
