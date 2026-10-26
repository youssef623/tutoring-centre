using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListTeachersHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task HandleAsync_PassesIncludeArchivedFlagThrough(bool includeArchived)
    {
        var readService = new FakeTeacherReadService();
        var handler = new ListTeachersHandler(readService);

        await handler.HandleAsync(new ListTeachersQuery(includeArchived), CancellationToken.None);

        Assert.Equal(includeArchived, readService.LastIncludeArchivedRequested);
    }

    [Fact]
    public async Task HandleAsync_ReturnsWhatTheReadServiceProvides()
    {
        var readService = new FakeTeacherReadService();
        readService.Teachers.Add(new TeacherDto(Guid.CreateVersion7(), "Amina Hassan", null, TeacherStatus.Active, null, null, 0, 0));
        var handler = new ListTeachersHandler(readService);

        var result = await handler.HandleAsync(new ListTeachersQuery(false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Amina Hassan", result.Value[0].FullName);
    }
}
