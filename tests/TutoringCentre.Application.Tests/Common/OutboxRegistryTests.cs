using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;

namespace TutoringCentre.Application.Tests.Common;

public sealed class OutboxRegistryTests
{
    [Fact]
    public void DuplicateRegistration_ThrowsBeforeStartup()
    {
        var services = new ServiceCollection();
        services.AddOutboxMessage<TestMessage, TestCommand>(message => new TestCommand(message.Id));
        Assert.Throws<InvalidOperationException>(() =>
            services.AddOutboxMessage<TestMessage, TestCommand>(message => new TestCommand(message.Id)));
    }

    [Fact]
    public void UnknownType_ResolvesFalse_AndRegisteredTypeBuildsOnlyItsCommand()
    {
        var services = new ServiceCollection();
        services.AddOutboxMessage<TestMessage, TestCommand>(message => new TestCommand(message.Id));
        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<OutboxRegistry>();
        Assert.False(registry.TryResolve("unknown", "{}", out var missing));
        Assert.Null(missing);
        var id = Guid.NewGuid();
        Assert.True(registry.TryResolve(TestMessage.TypeName, System.Text.Json.JsonSerializer.Serialize(new TestMessage(id)), out var found));
        Assert.Equal(new TestCommand(id), found!.Command);
    }

    private sealed record TestMessage(Guid Id) : IOutboxMessage
    {
        public static string TypeName => "test.registry";
    }
    private sealed record TestCommand(Guid Id) : ICommand<int>;
}
