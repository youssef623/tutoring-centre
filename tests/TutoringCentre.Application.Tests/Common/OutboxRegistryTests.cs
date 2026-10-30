using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;

namespace TutoringCentre.Application.Tests.Common;

public sealed class OutboxRegistryTests
{
    [Fact]
    public void DeferredType_IsKnownButCannotResolveUntilARealConsumerReplacesIt()
    {
        var services = new ServiceCollection();
        services.AddDeferredOutboxMessage<TestMessage>();
        using (var deferred = services.BuildServiceProvider())
        {
            var registry = deferred.GetRequiredService<OutboxRegistry>();
            Assert.True(registry.IsDeferred(TestMessage.TypeName));
            Assert.False(registry.TryResolve(TestMessage.TypeName, "{}", out _));
            Assert.False(registry.IsDeferred("unknown"));
        }
        services.AddOutboxMessage<TestMessage, TestCommand>(message => new(message.Id));
        using var installed = services.BuildServiceProvider();
        var active = installed.GetRequiredService<OutboxRegistry>();
        Assert.False(active.IsDeferred(TestMessage.TypeName));
        Assert.True(active.TryResolve(TestMessage.TypeName, "{\"id\":\"00000000-0000-0000-0000-000000000001\"}", out _));
        Assert.Throws<InvalidOperationException>(() => services.AddDeferredOutboxMessage<TestMessage>());
    }

    [Fact]
    public void DuplicateDeferral_ThrowsBeforeStartup()
    {
        var services = new ServiceCollection();
        services.AddDeferredOutboxMessage<TestMessage>();
        Assert.Throws<InvalidOperationException>(() => services.AddDeferredOutboxMessage<TestMessage>());
    }

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
