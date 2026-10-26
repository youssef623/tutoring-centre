namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// The application clock's time source in the Api tests (Day 53). Real time by default, so every earlier test runs
/// exactly as before; a test that depends on "today" or "in the past" fixes it with <see cref="FixAt"/> and the
/// fixture puts it back with <see cref="UseRealTime"/>. The collection runs its classes one at a time, so a fixed
/// time never leaks into a concurrently running test.
/// </summary>
public sealed class FixableTimeProvider : TimeProvider
{
    private DateTimeOffset? _fixedUtcNow;

    public void FixAt(DateTimeOffset utcNow) => _fixedUtcNow = utcNow.ToUniversalTime();

    public void UseRealTime() => _fixedUtcNow = null;

    public override DateTimeOffset GetUtcNow() => _fixedUtcNow ?? System.GetUtcNow();
}
