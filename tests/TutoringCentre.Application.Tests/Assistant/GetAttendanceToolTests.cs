using TutoringCentre.Application.Attendance.Contracts;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Assistant.Handles;
using TutoringCentre.Application.Assistant.Tools;
using TutoringCentre.Application.Assistant.Tools.GetAttendance;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Queries.GetAttendance;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class GetAttendanceToolTests
{
    [Theory] [InlineData(0)] [InlineData(1)] [InlineData(3)]
    public async Task ReturnsOnlyLocalFactsAndEmptyNote(int count)
    {
        var child = new PortalChild(Guid.NewGuid(), "Ahmed");
        var handler = new QueryHandler(child.Id, count);
        var actor = new FakeCurrentActor { Actor = new ParentActor(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()) };
        var services = new ServiceCollection().AddSingleton<ICurrentActor>(actor).AddSingleton<IQueryHandler<GetAttendanceQuery, AttendancePageDto>>(handler).BuildServiceProvider();
        var dispatcher = new Dispatcher(services, new FakeUnitOfWork(), NullLogger<Dispatcher>.Instance);
        var tool = new GetAttendanceTool(dispatcher, new TutoringCentre.Application.Assistant.AssistantCalendar(new FakeCentreTimeZoneProvider(), new FakeClock { UtcNow = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero) }));
        var args = ToolArguments.Parse(tool.Descriptor.Schema, count == 3 ? "{\"childHandle\":\"child_1\",\"from\":\"2026-08-29\",\"to\":\"2026-10-30\"}" : "{\"childHandle\":\"child_1\"}").Value;
        var result = await tool.ExecuteAsync(new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), HandleResolver.Mint([child])), args, default);
        Assert.True(result.IsSuccess);
        var facts = Assert.IsType<GetAttendanceToolResult>(result.Value.Data);
        Assert.Equal(count, facts.Records.Count);
        Assert.Equal(count == 0 ? GetAttendanceToolResult.EmptyNote : null, facts.Note);
        Assert.Equal(count == 3 ? "2026-08-29" : "2026-10-30", facts.From);
        Assert.Equal(31, handler.Query!.PageSize);
        Assert.DoesNotMatch(@"[0-9a-fA-F]{8}-[0-9a-fA-F-]{27,}", JsonSerializer.Serialize(facts));
        if(count > 0) Assert.Equal("12:00", facts.Records[0].StartsAt);
    }
    [Theory]
    [InlineData("{\"childHandle\":\"child_2\"}", "tool.handle_unknown")]
    [InlineData("{\"childHandle\":\"child_1\",\"from\":\"2026-01-01\",\"to\":\"2026-05-01\"}", "tool.invalid_arguments")]
    [InlineData("{\"childHandle\":\"child_1\",\"date\":\"2026-01-01\",\"to\":\"2026-01-02\"}", "tool.invalid_arguments")]
    public async Task RejectsBeforeQuery(string json, string code)
    {
        var tool = new GetAttendanceTool(null!, new TutoringCentre.Application.Assistant.AssistantCalendar(new FakeCentreTimeZoneProvider(), new FakeClock()));
        var result = await tool.ExecuteAsync(new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), HandleResolver.Mint([new(Guid.NewGuid(), "Ahmed")])), ToolArguments.Parse(tool.Descriptor.Schema, json).Value, default);
        Assert.Equal(code, result.Error!.Code);
    }
    private sealed class QueryHandler(Guid childId, int count) : IQueryHandler<GetAttendanceQuery, AttendancePageDto>
    {
        public GetAttendanceQuery? Query { get; private set; }
        public Task<Result<AttendancePageDto>> HandleAsync(GetAttendanceQuery query, CancellationToken ct)
        {
            Query = query;
            return Task.FromResult(Result<AttendancePageDto>.Success(new(Enumerable.Range(0, count).Select(index => new AttendanceHistoryItemDto(Guid.NewGuid(), Guid.NewGuid(), new(2026, 10, 30), new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero), Guid.NewGuid(), "Math", "Math", childId, "Ahmed", "present", "manual", 0)).ToArray(), null)));
        }
    }
}
