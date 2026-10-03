# src/TutoringCentre.Infrastructure/Time/SystemClock.cs

## Purpose
The `IClock` adapter. It takes "now" from `TimeProvider` and does time-zone conversions with the OS/ICU time-zone database, using IANA ids such as `Africa/Cairo` (line 5).

## Where it fits
Infrastructure/Time, `public sealed`. Registered singleton as `IClock` (`DependencyInjection.cs:28`), with `TimeProvider.System` injected. Used by `TimestampInterceptor`. Tested by `tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs` with `FakeTimeProvider`.

## Walkthrough
- **Lines 10–14:** the constructor takes `TimeProvider`. In tests this is `FakeTimeProvider`.
- **Line 16:** `UtcNow => _timeProvider.GetUtcNow()`.
- **Lines 18–22, `ToLocal`:** `FindSystemTimeZoneById`, then `TimeZoneInfo.ConvertTime(utc, zone).DateTime`. `.DateTime` of a `DateTimeOffset` has `Kind = Unspecified`, matching the port's doc.
- **Lines 24–30, `FromLocal`:**
  1. Find the zone.
  2. `DateTime.SpecifyKind(local, Unspecified)`, so a `Local`/`Utc` kind passed in doesn't change the meaning.
  3. `zone.GetUtcOffset(wallClock)`.
  4. `new DateTimeOffset(wallClock, offset).ToUniversalTime()`.

## Concepts used
- **`TimeProvider`** (.NET 8+): an abstraction over system time, designed for testing.
- **IANA vs Windows time-zone ids.**
- **Wall-clock time vs instant.**

## Data and control flow
Local wall clock + zone → offset → UTC instant; or UTC instant + zone → wall clock.

## Configuration and environment
Relies on the OS/ICU tz database being present. Minimal container images (e.g. Alpine without `tzdata`/ICU) can lack it. That is general .NET behaviour, not repo code.

## Gotchas and issues
These were verified with a probe on SDK 10.0.112 / Linux:
- **DST gap (non-existent local time).** `FromLocal(2027-04-30 00:30, Cairo)` returns `2027-04-29T22:30Z`, which converts back to **01:30**, not 00:30. A schedule entered for an invalid time silently shifts.
- **DST overlap (ambiguous local time).** `FromLocal(2027-10-28 23:30, Cairo)` returns `21:30Z`, i.e. the *standard* offset (+2), the later of the two possible instants.
- **Unknown zone ids** throw `TimeZoneNotFoundException`. No `Result` is involved, because a bad id here is a bug: `Centre.Create` already validated it.
- Tests cover only normal winter (+2) and summer (+3) cases.

## Related files
- [IClock.cs](../../TutoringCentre.Application/Common/Ports/IClock.cs.md)
- [SystemClockTests.cs](../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs.md)
- [Centre.cs](../../TutoringCentre.Domain/Centres/Centre.cs.md)
