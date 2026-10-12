using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListStudentsHandlerTests
{
    [Fact]
    public async Task HandleAsync_PassesCriteriaThroughAndReturnsThePage()
    {
        var readService = new FakeStudentReadService();
        var item = new StudentListItemDto(Guid.CreateVersion7(), "Youssef Omar", null, StudentStatus.Active, null, null);
        readService.Page = new StudentPageDto([item], "next-cursor");
        var handler = new ListStudentsHandler(readService);

        var result = await handler.HandleAsync(new ListStudentsQuery("omar", false, "cursor", 25), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(readService.Page, result.Value);
    }
}
