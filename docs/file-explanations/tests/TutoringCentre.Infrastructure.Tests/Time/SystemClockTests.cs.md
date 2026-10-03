# tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs

## Purpose
Tests `SystemClock`:
- `UtcNow` comes from the injected `TimeProvider`;
- Cairo local-to-UTC conversion uses +2 in winter and +3 in summer;
- both conversions round-trip.

## Where it fits
Infrastructure.Tests. It tests `Infrastructure/Time/SystemClock.cs` with `Microsoft.Extensions.Time.Testing.FakeTimeProvider`.

## Walkthrough
- **Lines 10–22:** `FakeTimeProvider(now)` → `clock.UtcNow == now`.
- **Lines 24–36:** 2027-01-15 17:00 local → 15:00Z, offset zero, and `ToLocal` gives back 17:00.
- **Lines 38–49:** 2027-07-15 17:00 local → 14:00Z, which proves Cairo DST (+3) is applied.

## Concepts used
- **Fake time.**
- **Round-trip assertions.**

## Data and control flow
Not applicable.

## Configuration and environment
Requires the OS tz database with current Egyptian DST rules. Egypt reinstated DST in 2023; an outdated tz database would fail the summer test.

## Gotchas and issues
- **No DST-boundary tests.** Gap and overlap times aren't tested; see `SystemClock.cs.md` for the verified behaviour.
- **No unknown-zone test.**

## Related files
- [SystemClock.cs](../../../src/TutoringCentre.Infrastructure/Time/SystemClock.cs.md)
- [IClock.cs](../../../src/TutoringCentre.Application/Common/Ports/IClock.cs.md)
