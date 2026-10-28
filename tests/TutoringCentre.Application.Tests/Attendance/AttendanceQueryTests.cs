using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Queries.GetAttendance;
using TutoringCentre.Application.Attendance.Queries.GetSessionAttendance;
using TutoringCentre.Application.Attendance.Queries.ListAttendanceSessions;
using TutoringCentre.Application.Tests.Fakes;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class AttendanceQueryTests
{
    [Theory]
    [InlineData("all", 2)]
    [InlineData("own", 1)]
    [InlineData("none", 0)]
    public async Task Sessions_ApplyScopeAndCounts(string kind, int count)
    {
        var fixture = new AttendanceHandlerFixture();
        fixture.Scope.Scope = Scope(kind);
        var reads = new Reads();
        var result = await new ListAttendanceSessionsHandler(fixture.Roster, fixture.Scope, reads, new FakeCentreTimeZoneProvider(), fixture.Clock)
            .HandleAsync(new(fixture.Session.LocalDate), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(count, result.Value.Items.Count);
        if (count > 0)
        {
            var own = Assert.Single(result.Value.Items, item => item.SessionId == fixture.Session.SessionId);
            Assert.Equal(3, own.ExpectedCount);
            Assert.Equal("open", own.Window);
        }
        else { Assert.Empty(fixture.Roster.Calls); }
    }

    [Fact]
    public async Task DefaultDate_UsesCairoAfterMidnight()
    {
        var fixture = new AttendanceHandlerFixture();
        fixture.Clock.UtcNow = new(2026, 12, 4, 22, 30, 0, TimeSpan.Zero);
        var result = await new ListAttendanceSessionsHandler(fixture.Roster, fixture.Scope, new Reads(), new FakeCentreTimeZoneProvider(), fixture.Clock)
            .HandleAsync(new(), CancellationToken.None);
        Assert.Equal(new DateOnly(2026, 12, 5), result.Value.Date);
        Assert.Equal("Africa/Cairo", result.Value.TimeZoneId);
        Assert.Single(result.Value.Items);
        Assert.Equal("not_open", result.Value.Items[0].Window);
    }

    [Theory]
    [InlineData(-31, "not_open")]
    [InlineData(-30, "open")]
    [InlineData(211, "closed")]
    public async Task Sessions_WindowUsesClock(int minutes, string expected)
    {
        var fixture = new AttendanceHandlerFixture();
        fixture.Clock.UtcNow = fixture.Session.StartsAt.AddMinutes(minutes);
        var result = await new ListAttendanceSessionsHandler(fixture.Roster, fixture.Scope, new Reads(), new FakeCentreTimeZoneProvider(), fixture.Clock)
            .HandleAsync(new(fixture.Session.LocalDate), CancellationToken.None);
        Assert.Equal(expected, Assert.Single(result.Value.Items).Window);
    }

    [Fact]
    public async Task Roster_MergesOnlyExpectedStudents_WithNullForUnmarked()
    {
        var fixture = new AttendanceHandlerFixture();
        var reads = new Reads();
        reads.Records.Add(new(fixture.Students[0].StudentId, new(Guid.NewGuid(), "present", "manual", fixture.Clock.UtcNow, "Marker", 0, 55)));
        reads.Records.Add(new(Guid.NewGuid(), reads.Records[0].Record));
        var result = await new GetSessionAttendanceHandler(fixture.Roster, fixture.Scope, reads, new FakeCentreTimeZoneProvider(), fixture.Clock)
            .HandleAsync(new(fixture.Session.SessionId), CancellationToken.None);
        Assert.Equal(3, result.Value.Items.Count);
        Assert.Equal(55u, result.Value.Items[0].Record!.Version);
        Assert.Null(result.Value.Items[1].Record);
    }

    [Fact]
    public async Task Roster_NotOwnedAndUnknown_AreNotFound()
    {
        var fixture = new AttendanceHandlerFixture();
        var handler = new GetSessionAttendanceHandler(fixture.Roster, fixture.Scope, new Reads(), new FakeCentreTimeZoneProvider(), fixture.Clock);
        foreach (var id in new[] { fixture.OtherSession.SessionId, Guid.NewGuid() })
        {
            var result = await handler.HandleAsync(new(id), CancellationToken.None);
            Assert.Equal("attendance.session_not_found", result.Error!.Code);
        }
    }

    [Theory]
    [InlineData("all")]
    [InlineData("own")]
    [InlineData("none")]
    public async Task History_MapsScope(string kind)
    {
        var reads = new Reads();
        var scope = new FakeTeachingScope { Scope = Scope(kind) };
        var result = await new GetAttendanceHandler(scope, reads).HandleAsync(new(null, null, new(2026, 12, 1), new(2026, 12, 5)), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(kind == "all", reads.Access!.IsAll);
        Assert.Equal(kind == "own" ? AttendanceHandlerFixture.TeacherId : (Guid?)null, reads.Access.TeacherId);
        Assert.Empty(result.Value.Items);
    }

    [Theory]
    [InlineData(-1, 50, null, false)]
    [InlineData(92, 50, null, true)]
    [InlineData(93, 50, null, false)]
    [InlineData(1, 0, null, false)]
    [InlineData(1, 101, null, false)]
    [InlineData(1, 50, "bad", false)]
    public void History_ValidatesRangeSizeAndCursor(int days, int size, string? cursor, bool valid)
    {
        var from = new DateOnly(2026, 12, 1);
        Assert.Equal(valid, new GetAttendanceValidator().Validate(new GetAttendanceQuery(null, null, from, from.AddDays(days), cursor, size)).IsValid);
    }

    [Theory]
    [InlineData(-367, false)]
    [InlineData(-366, true)]
    [InlineData(366, true)]
    [InlineData(367, false)]
    public async Task Sessions_ValidateCentreDate(int days, bool valid)
    {
        var fixture = new AttendanceHandlerFixture();
        var validator = new ListAttendanceSessionsValidator(new FakeCentreTimeZoneProvider(), fixture.Clock, fixture.Actor);
        Assert.Equal(valid, (await validator.ValidateAsync(new ListAttendanceSessionsQuery(fixture.Session.LocalDate.AddDays(days)))).IsValid);
    }

    [Fact]
    public void Cursor_RoundTripsAndRejectsOutOfRangeValues()
    {
        var date = new DateOnly(2026, 12, 5);
        var time = new DateTimeOffset(2026, 12, 5, 16, 0, 0, TimeSpan.Zero);
        var id = Guid.NewGuid();
        Assert.True(AttendanceCursor.TryDecode(AttendanceCursor.Encode(date, time, id), out var decoded));
        Assert.Equal((date, time, id), decoded);
        Assert.False(AttendanceCursor.TryDecode(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"999999999|999999999999999999|{id:N}")), out _));
    }

    private static TeachingScope Scope(string kind) => kind switch
    {
        "all" => TeachingScope.All,
        "own" => TeachingScope.Own(AttendanceHandlerFixture.TeacherId),
        _ => TeachingScope.None,
    };

    private sealed class Reads : IAttendanceReadService
    {
        public AttendanceAccess? Access { get; private set; }
        public List<SessionAttendanceRecordDto> Records { get; } = [];
        public Task<IReadOnlyList<SessionAttendanceRecordDto>> ListForSessionAsync(Guid sessionId, CancellationToken ct) => Task.FromResult<IReadOnlyList<SessionAttendanceRecordDto>>(Records);
        public Task<IReadOnlyDictionary<Guid, int>> CountForSessionsAsync(IReadOnlyCollection<Guid> sessionIds, CancellationToken ct) => Task.FromResult<IReadOnlyDictionary<Guid, int>>(new Dictionary<Guid, int>());
        public Task<AttendancePageDto> ListAsync(AttendanceFilter filter, AttendanceAccess access, string? cursor, int pageSize, CancellationToken ct)
        {
            Access = access;
            return Task.FromResult(new AttendancePageDto([], null));
        }
    }
}
