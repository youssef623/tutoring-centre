using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class GetTeacherHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var readService = new FakeTeacherReadService();
        var handler = new GetTeacherHandler(readService);

        var result = await handler.HandleAsync(new GetTeacherQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_KnownId_ReturnsDto()
    {
        var teacherId = Guid.CreateVersion7();
        var readService = new FakeTeacherReadService();
        readService.Teachers.Add(new TeacherDto(teacherId, "Amina Hassan", null, TeacherStatus.Active, null, null, 0, 0));
        var handler = new GetTeacherHandler(readService);

        var result = await handler.HandleAsync(new GetTeacherQuery(teacherId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Amina Hassan", result.Value.FullName);
    }
}
