using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class GetStudentHandlerTests
{
    [Fact]
    public async Task HandleAsync_Known_ReturnsDetail()
    {
        var readService = new FakeStudentReadService();
        var detail = new StudentDetailDto(Guid.CreateVersion7(), "Youssef Omar", null, null, StudentStatus.Active, 0, []);
        readService.Details.Add(detail);
        var handler = new GetStudentHandler(readService);

        var result = await handler.HandleAsync(new GetStudentQuery(detail.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(detail, result.Value);
    }

    [Fact]
    public async Task HandleAsync_Unknown_ReturnsStudentNotFound()
    {
        var handler = new GetStudentHandler(new FakeStudentReadService());

        var result = await handler.HandleAsync(new GetStudentQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
    }
}
