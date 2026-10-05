# src/TutoringCentre.Application/Common/Ports/IClock.cs

## Purpose

Port for the current time and for converting between UTC instants and local wall-clock time in an IANA time zone.

## Where It Fits

Application/Common/Ports. Implemented by `Infrastructure/Time/SystemClock`; consumed by `TimestampInterceptor`. `ToLocal`/`FromLocal` have no product callers yet (only tests).

## Walkthrough

Members: `UtcNow` (current instant, UTC), `ToLocal(DateTimeOffset utc, string timeZoneId)` -> `DateTime` of kind Unspecified, `FromLocal(DateTime local, string timeZoneId)` -> `DateTimeOffset` with zero offset. Doc: 'the only source of now'. Centres carry a `TimeZoneId` (e.g. `Africa/Cairo`), anticipating schedules stored in UTC and shown locally.

## Concepts Used

### Time abstraction and time zones

#### What it means

Reading `DateTime.UtcNow` directly makes code untestable. A clock *port* (`IClock`) supplies time; tests substitute a fake. IANA zone ids (`Africa/Cairo`) resolve through the OS time-zone database and handle daylight saving.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning](../../../../../PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

The port.

#### How it works here

Interface.

#### Why it matters here

Lets tests inject `FakeTimeProvider` via `SystemClock`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Unused members are speculative API surface (status: implemented, tested, unused).

## Related Files

- [`src/TutoringCentre.Infrastructure/Time/SystemClock.cs`](../../../TutoringCentre.Infrastructure/Time/SystemClock.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs`](../../../TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs`](../../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs.md)
