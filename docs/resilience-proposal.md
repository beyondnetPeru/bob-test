# API Resilience Proposal (Polly)

## Current State

- The API currently does not perform outbound HTTP calls.
- Most operations are EF Core database reads/writes against SQLite.
- A resilient HTTP client baseline has now been added (`external-api`) with Polly retry and circuit breaker policies.
- A typed client abstraction (`IExternalDependencyClient`) is now wired to that resilient pipeline for upcoming integrations.

## Where to Apply Retry

Use retries only for transient faults and idempotent operations.

1. Outbound HTTP GET/HEAD requests to external services

- Apply: yes
- Reason: network glitches, DNS hiccups, `5xx`, and `429` are often transient.
- Policy: exponential backoff retry (already configured).

2. Outbound HTTP POST/PUT/PATCH/DELETE

- Apply: conditional
- Reason: retries can duplicate side effects.
- Requirement: idempotency key and server-side deduplication, or explicit operation idempotency guarantee.

3. EF Core reads (`GetByIdAsync`, `GetAllAsync`)

- Apply: optional/limited
- Reason: can help with transient DB transport issues in remote DB setups.
- Note: low value with local SQLite.

4. EF Core writes (`CommitAsync`)

- Apply: do not apply by default
- Reason: commit retries can create duplicate writes when failure happens after server-side commit but before client acknowledgment.
- Recommendation: use optimistic concurrency and idempotency patterns instead of blind retry.

## Where to Apply Circuit Breaker

1. Outbound HTTP integrations (payment/ERP/catalog/identity providers)

- Apply: yes
- Reason: avoids cascading failure and thread starvation when dependency is degraded.
- Policy: open circuit after repeated transient failures (already configured).

2. Database calls

- Apply: generally no (at app layer)
- Reason: DB is a core dependency; circuit breaker here often amplifies outage behavior.
- Better: health checks + infrastructure-level recovery + connection pooling tuning.

## Recommended Rollout Plan

1. Phase 1 (implemented)

- Add Polly dependencies.
- Register centralized retry + circuit breaker policies for named `HttpClient` (`external-api`).
- Make policy thresholds configurable in `appsettings.json`.

2. Phase 2

- Introduce additional typed clients per dependency (for example: `ICatalogClient`, `IIdentityClient`) and attach per-client policies.
- Add per-endpoint overrides (stricter retry for GET, conservative for POST).

3. Phase 3

- Add telemetry:
- Retry count metric by dependency
- Circuit open/half-open/closed state metric
- Alerting on sustained open circuits

4. Phase 4

- Add fallback behavior for non-critical reads (cache/stale response).
- Add idempotency keys for write endpoints before enabling retries on non-idempotent operations.

## Proposed Default Settings

- `RetryCount`: 3
- `RetryBackoffBaseSeconds`: 0.5
- `CircuitBreakerFailuresAllowedBeforeBreaking`: 5
- `CircuitBreakerBreakDurationSeconds`: 30

## Code References

- Resilience registration: `apps/api/Infrastructure/Resilience/DependencyInjection.cs`
- Resilience options: `apps/api/Infrastructure/Resilience/ResilienceOptions.cs`
- Typed external client abstraction: `apps/api/Application/Abstractions/Services/IExternalDependencyClient.cs`
- Typed external client implementation: `apps/api/Infrastructure/Services/ExternalDependencyClient.cs`
- Startup wiring: `apps/api/Program.cs`
- Config values: `apps/api/appsettings.json`
