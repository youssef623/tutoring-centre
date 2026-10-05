# src/TutoringCentre.Infrastructure/Time/SystemClock.cs

## Purpose

Real implementation of `IClock` based on `TimeProvider` and the OS time-zone database.

## Where It Fits

Infrastructure/Time. Registered singleton. Used by `TimestampInterceptor`.

## Walkthrough

`UtcNow => _timeProvider.GetUtcNow()` (16). `ToLocal` (18): `TimeZoneInfo.FindSystemTimeZoneById`, `ConvertTime(utc, zone).DateTime`. `FromLocal` (24): `DateTime.SpecifyKind(local, Unspecified)`, `zone.GetUtcOffset(wallClock)`, `new DateTimeOffset(wallClock, offset).ToUniversalTime()`. Verified by `SystemClockTests`: Cairo 17:00 is 15:00 UTC in January (+2) and 14:00 UTC in July (+3). Unknown ids throw `TimeZoneNotFoundException`; ambiguous/nonexistent DST local times are not specially handled.

## Concepts Used

### Time abstraction and time zones

#### What it means

Reading `DateTime.UtcNow` directly makes code untestable. A clock *port* (`IClock`) supplies time; tests substitute a fake. IANA zone ids (`Africa/Cairo`) resolve through the OS time-zone database and handle daylight saving.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning](../../../../PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Clock adapter.

#### How it works here

Whole class.

#### Why it matters here

Deterministic tests via `FakeTimeProvider`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No handling for daylight-saving gaps/overlaps; method surface unused by product code.

## Related Files

- [`src/TutoringCentre.Application/Common/Ports/IClock.cs`](../../TutoringCentre.Application/Common/Ports/IClock.cs.md)
- [`tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs`](../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs.md)
- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](../DependencyInjection.cs.md)
