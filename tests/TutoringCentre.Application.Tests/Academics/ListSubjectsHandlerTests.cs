using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListSubjectsHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task HandleAsync_PassesIncludeArchivedFlagThrough(bool includeArchived)
    {
        var readService = new FakeSubjectReadService();
        var handler = new ListSubjectsHandler(readService);

        await handler.HandleAsync(new ListSubjectsQuery(includeArchived), CancellationToken.None);

        Assert.Equal(includeArchived, readService.LastIncludeArchivedRequested);
    }

    [Fact]
    public async Task HandleAsync_ReturnsWhatTheReadServiceProvides()
    {
        var readService = new FakeSubjectReadService();
        readService.Subjects.Add(new SubjectDto(Guid.CreateVersion7(), "Mathematics", SubjectStatus.Active, 0));
        var handler = new ListSubjectsHandler(readService);

        var result = await handler.HandleAsync(new ListSubjectsQuery(false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Mathematics", result.Value[0].Name);
    }
}
