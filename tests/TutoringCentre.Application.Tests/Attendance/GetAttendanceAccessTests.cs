using TutoringCentre.Application.Attendance.Contracts;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Queries.GetAttendance;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
namespace TutoringCentre.Application.Tests.Attendance;
public sealed class GetAttendanceAccessTests
{
    [Theory] [InlineData(false)] [InlineData(true)]
    public async Task PortalUsesResolvedChildrenAndHidesUnknownIds(bool student)
    {
        var own = Guid.NewGuid(); var second = Guid.NewGuid();
        var scope = new Scope(student ? [own] : [own, second]); var reads = new Reads();
        var actor = new FakeCurrentActor { Actor = student ? new StudentActor(Guid.NewGuid(), Guid.NewGuid(), own) : new ParentActor(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()) };
        var handler = new GetAttendanceHandler(new FakeTeachingScope { Scope = TeachingScope.All }, reads, actor, scope);
        var query = new GetAttendanceQuery(null, Guid.NewGuid(), new(2026, 10, 1), new(2026, 10, 31));
        Assert.True((await handler.HandleAsync(query, default)).IsSuccess);
        Assert.False(reads.Access!.IsAll);
        Assert.Equal(scope.Ids.Order(), reads.Access.StudentIds!.Order());
        var rejected = await handler.HandleAsync(query with { StudentId = Guid.NewGuid() }, default);
        Assert.Equal("attendance.not_found", rejected.Error!.Code);
        Assert.Equal(1, reads.Calls);
    }
    [Fact]
    public void ChildrenScopeCopiesInputAndNeverGrantsAll()
    {
        var id = Guid.NewGuid(); var input = new List<Guid> { id }; var access = AttendanceAccess.Children(input);
        input.Clear(); Assert.Equal(id, Assert.Single(access.StudentIds!)); Assert.False(access.IsAll);
        Assert.Empty(AttendanceAccess.Children([]).StudentIds!);
        Assert.Null(AttendanceAccess.None.StudentIds);
    }
    private sealed class Scope(IReadOnlyList<Guid> ids) : IPortalScopeResolver
    {
        public IReadOnlyList<Guid> Ids { get; } = ids;
        public Task<IReadOnlySet<Guid>> ResolveAsync(IPortalActor actor, CancellationToken ct) => Task.FromResult<IReadOnlySet<Guid>>(Ids.ToHashSet());
    }
    private sealed class Reads : IAttendanceReadService
    {
        public AttendanceAccess? Access { get; private set; }
        public int Calls { get; private set; }
        public Task<AttendancePageDto> ListAsync(AttendanceFilter filter, AttendanceAccess access, string? cursor, int pageSize, CancellationToken ct) { Access = access; Calls++; return Task.FromResult(new AttendancePageDto([], null)); }
        public Task<IReadOnlyList<SessionAttendanceRecordDto>> ListForSessionAsync(Guid sessionId, CancellationToken ct) => throw new NotSupportedException();
        public Task<IReadOnlyDictionary<Guid, int>> CountForSessionsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) => throw new NotSupportedException();
    }
}
