using FluentAssertions;
using Infrastructure.Resilience;
using Microsoft.Extensions.Logging.Abstractions;
using Polly.CircuitBreaker;
using System.Net;

namespace Api.Tests.Infrastructure.Resilience;

public class ResiliencePolicyFactoryTests
{
    [Test]
    public async Task BuildRetryPolicy_RetriesTransientFailures_ThenSucceeds()
    {
        var options = new ResilienceOptions
        {
            RetryCount = 2,
            RetryBackoffBaseSeconds = 0
        };

        var policy = ResiliencePolicyFactory.BuildRetryPolicy(options, NullLogger.Instance, "dependency-a");

        var attempts = 0;

        var response = await policy.ExecuteAsync(() =>
        {
            attempts++;

            if (attempts <= 2)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        attempts.Should().Be(3);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task BuildCircuitBreakerPolicy_OpensAfterThreshold_AndBreaksSubsequentCalls()
    {
        var options = new ResilienceOptions
        {
            CircuitBreakerFailuresAllowedBeforeBreaking = 2,
            CircuitBreakerBreakDurationSeconds = 5
        };

        var policy = ResiliencePolicyFactory.BuildCircuitBreakerPolicy(options, NullLogger.Instance, "dependency-b");

        await policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));
        await policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));

        Func<Task> act = async () => await policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        await act.Should().ThrowAsync<BrokenCircuitException<HttpResponseMessage>>();
    }
}
