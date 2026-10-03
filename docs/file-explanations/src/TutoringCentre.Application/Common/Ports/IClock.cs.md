# src/TutoringCentre.Application/Common/Ports/IClock.cs

## Purpose
The port that is "the only source of 'now' and of local-time conversion" (line 3). It exposes `UtcNow`, `ToLocal(utc, timeZoneId)` and `FromLocal(local, timeZoneId)` for IANA zones.

## Where it fits
Application/Common/Ports.
- **Implemented by:** `Infrastructure/Time/SystemClock.cs`.
- **Registered:** singleton in `Infrastructure/DependencyInjection.cs:28`.
- **Consumers:** `Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs` (for `UtcNow`). Future Application handlers dealing with schedules and attendance in each centre's `TimeZoneId`.

## Walkthrough
- **Line 7:** `DateTimeOffset UtcNow { get; }`, the current instant in UTC.
- **Line 10:** `DateTime ToLocal(DateTimeOffset utc, string timeZoneId)` returns wall-clock time with `DateTimeKind.Unspecified`.
- **Line 13:** `DateTimeOffset FromLocal(DateTime local, string timeZoneId)` returns the instant, with offset zero.

## Concepts used
- **Port (hexagonal architecture):** an interface owned by the inner layer.
- **Testable time:** code depends on `IClock`, not `DateTime.Now`, so tests can fix the time.

## Data and control flow
Not applicable (interface).

## Configuration and environment
None.

## Gotchas and issues
- The contract doesn't say how DST gaps (non-existent local times) and overlaps (ambiguous times) are handled. The implementation silently picks the standard offset (verified; see `SystemClock.cs.md`).
- Why a custom port when .NET has `TimeProvider`? It adds time-zone conversion and keeps Application free of a specific implementation. `SystemClock` wraps `TimeProvider`.

## Related files
- [SystemClock.cs](../../../TutoringCentre.Infrastructure/Time/SystemClock.cs.md)
- [TimestampInterceptor.cs](../../../TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs.md)
