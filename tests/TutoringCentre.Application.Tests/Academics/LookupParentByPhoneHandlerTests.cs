using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class LookupParentByPhoneHandlerTests
{
    [Fact]
    public async Task HandleAsync_FoundByDifferentSpelling_ReturnsTheParent()
    {
        var existing = new ParentDto(Guid.CreateVersion7(), "Omar Hassan", "+201012345678", 0);
        var readService = new FakeParentReadService();
        readService.ByPhone.Add(existing);
        var handler = new LookupParentByPhoneHandler(readService);

        var result = await handler.HandleAsync(new LookupParentByPhoneQuery("0101 234 5678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(existing, result.Value.Parent);
    }

    [Fact]
    public async Task HandleAsync_NoParentWithThisPhone_SucceedsWithNull()
    {
        var readService = new FakeParentReadService();
        var handler = new LookupParentByPhoneHandler(readService);

        var result = await handler.HandleAsync(new LookupParentByPhoneQuery("01012345678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Parent);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()
    {
        var readService = new FakeParentReadService();
        var handler = new LookupParentByPhoneHandler(readService);

        var result = await handler.HandleAsync(new LookupParentByPhoneQuery("12345"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("phone.invalid", result.Error!.Code);
        Assert.Contains("phone", result.Error.Fields!.Keys);
    }
}
