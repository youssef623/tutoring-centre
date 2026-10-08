using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class GetSubjectHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var readService = new FakeSubjectReadService();
        var handler = new GetSubjectHandler(readService);

        var result = await handler.HandleAsync(new GetSubjectQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_KnownId_ReturnsDto()
    {
        var subjectId = Guid.CreateVersion7();
        var readService = new FakeSubjectReadService();
        readService.Subjects.Add(new SubjectDto(subjectId, "Mathematics", SubjectStatus.Active, 0));
        var handler = new GetSubjectHandler(readService);

        var result = await handler.HandleAsync(new GetSubjectQuery(subjectId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Mathematics", result.Value.Name);
    }
}
