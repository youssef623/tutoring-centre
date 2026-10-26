using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Security;

public sealed class CorrelationContextTests
{
    [Fact]
    public void CorrelationId_ByDefault_IsNull()
    {
        var context = new CorrelationContext();

        Assert.Null(context.CorrelationId);
    }

    [Fact]
    public void Set_ThenCorrelationId_ReturnsTheSetValue()
    {
        var context = new CorrelationContext();

        context.Set("abc-123");

        Assert.Equal("abc-123", context.CorrelationId);
    }

    [Fact]
    public void Set_CalledTwice_ThrowsInvalidOperationException()
    {
        var context = new CorrelationContext();
        context.Set("first-id");

        Assert.Throws<InvalidOperationException>(() => context.Set("second-id"));
    }
}
