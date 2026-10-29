namespace TutoringCentre.Application.Common.Messaging;

public interface IOutboxMessage
{
    static abstract string TypeName { get; }
}

/// <summary>Adds deferred work to the current command's unit of work without saving it.</summary>
public interface IOutbox
{
    void Enqueue<TMessage>(TMessage message) where TMessage : class, IOutboxMessage;
}
