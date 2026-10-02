using Microsoft.Extensions.Time.Testing;
using TutoringCentre.Infrastructure.Time;

namespace TutoringCentre.Infrastructure.Tests.Time;

public sealed class SystemClockTests
{
    private const string Cairo = "Africa/Cairo";

    [Fact]
    public void UtcNow_ReturnsTheTimeProvidersTime()
    {
        // Arrange
        var now = new DateTimeOffset(2027, 1, 15, 10, 30, 0, TimeSpan.Zero);
        var clock = new SystemClock(new FakeTimeProvider(now));

        // Act
        var utcNow = clock.UtcNow;

        // Assert
        Assert.Equal(now, utcNow);
    }

    [Fact]
    public void FromLocal_InCairoWinter_UsesUtcPlusTwo()
    {
        var clock = new SystemClock(new FakeTimeProvider());
        var localFivePm = new DateTime(2027, 1, 15, 17, 0, 0);

        var utc = clock.FromLocal(localFivePm, Cairo);

        Assert.Equal(new DateTimeOffset(2027, 1, 15, 15, 0, 0, TimeSpan.Zero), utc);
        Assert.Equal(TimeSpan.Zero, utc.Offset);
        // The reverse conversion returns the original wall-clock time.
        Assert.Equal(localFivePm, clock.ToLocal(utc, Cairo));
    }

    [Fact]
    public void FromLocal_InCairoSummer_UsesUtcPlusThree()
    {
        var clock = new SystemClock(new FakeTimeProvider());
        var localFivePm = new DateTime(2027, 7, 15, 17, 0, 0);

        var utc = clock.FromLocal(localFivePm, Cairo);

        Assert.Equal(new DateTimeOffset(2027, 7, 15, 14, 0, 0, TimeSpan.Zero), utc);
        Assert.Equal(TimeSpan.Zero, utc.Offset);
        Assert.Equal(localFivePm, clock.ToLocal(utc, Cairo));
    }
}
