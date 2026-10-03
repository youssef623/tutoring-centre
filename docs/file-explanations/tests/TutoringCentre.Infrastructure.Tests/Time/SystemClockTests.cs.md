# tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs

## Purpose

Unit tests for `SystemClock` using `FakeTimeProvider`.

## Where It Fits

Infrastructure.Tests/Time (no database).

## Walkthrough

`UtcNow_ReturnsTheTimeProvidersTime`: fake provider at 2027-01-15 10:30Z -> `UtcNow` equal. `FromLocal_InCairoWinter_UsesUtcPlusTwo`: 2027-01-15 17:00 local -> 15:00Z, zero offset, and `ToLocal` round-trips. `FromLocal_InCairoSummer_UsesUtcPlusThree`: 2027-07-15 17:00 -> 14:00Z. Proves daylight saving is handled by the zone database. Not tested: DST gap/overlap, unknown zone.

## Concepts Used

### Time abstraction and time zones

#### What it means

Reading `DateTime.UtcNow` directly makes code untestable. A clock *port* (`IClock`) supplies time; tests substitute a fake. IANA zone ids (`Africa/Cairo`) resolve through the OS time-zone database and handle daylight saving.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning](../../../../PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Clock abstraction under test.

#### How it works here

All tests.

#### Why it matters here

Deterministic time and DST proof.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Depends on the OS having the Cairo zone and its DST rules (Egypt's DST rules changed in 2023; results for 2027 depend on tzdata).

## Related Files

- [`src/TutoringCentre.Infrastructure/Time/SystemClock.cs`](../../../src/TutoringCentre.Infrastructure/Time/SystemClock.cs.md)
- [`src/TutoringCentre.Application/Common/Ports/IClock.cs`](../../../src/TutoringCentre.Application/Common/Ports/IClock.cs.md)
