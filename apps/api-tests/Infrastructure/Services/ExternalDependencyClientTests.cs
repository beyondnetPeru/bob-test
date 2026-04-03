using Application.Abstractions.Services;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Text;

namespace Api.Tests.Infrastructure.Services;

public class ExternalDependencyClientTests
{
    [Test]
    public async Task IsAvailableAsync_ReturnsTrue_WhenResponseIsSuccess()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok", Encoding.UTF8, "text/plain")
            });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test")
        };

        IExternalDependencyClient client = new ExternalDependencyClient(httpClient, NullLogger<ExternalDependencyClient>.Instance);

        var result = await client.IsAvailableAsync("/health");

        result.Should().BeTrue();
    }

    [Test]
    public async Task IsAvailableAsync_ReturnsFalse_WhenResponseIsFailure()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test")
        };

        IExternalDependencyClient client = new ExternalDependencyClient(httpClient, NullLogger<ExternalDependencyClient>.Instance);

        var result = await client.IsAvailableAsync("/health");

        result.Should().BeFalse();
    }

    [Test]
    public async Task IsAvailableAsync_ReturnsFalse_WhenRequestThrows()
    {
        var handler = new ThrowingHttpMessageHandler();

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test")
        };

        IExternalDependencyClient client = new ExternalDependencyClient(httpClient, NullLogger<ExternalDependencyClient>.Instance);

        var result = await client.IsAvailableAsync("/health");

        result.Should().BeFalse();
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(responder(request));
    }

    private sealed class ThrowingHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            throw new HttpRequestException("Simulated transient failure");
    }
}
