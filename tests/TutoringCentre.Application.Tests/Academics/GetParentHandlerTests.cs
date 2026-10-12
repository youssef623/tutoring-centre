using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Queries.GetParent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class GetParentHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var readService = new FakeParentReadService();
        var handler = new GetParentHandler(readService);

        var result = await handler.HandleAsync(new GetParentQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_KnownId_ReturnsDetail()
    {
        var detail = new ParentDetailDto(Guid.CreateVersion7(), "Omar Hassan", "+201012345678", 0, []);
        var readService = new FakeParentReadService();
        readService.Details.Add(detail);
        var handler = new GetParentHandler(readService);

        var result = await handler.HandleAsync(new GetParentQuery(detail.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(detail, result.Value);
    }
}
