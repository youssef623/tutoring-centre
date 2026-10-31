using TutoringCentre.Application.Common.Messaging;
namespace TutoringCentre.Application.Tests.Fakes;
public sealed class FakeOutbox : IOutbox
{
    public List<object> Messages { get; } = [];
    public void Enqueue<TMessage>(TMessage message) where TMessage : class, IOutboxMessage => Messages.Add(message);
}
