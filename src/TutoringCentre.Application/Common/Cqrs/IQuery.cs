namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>A read that never changes state, producing <typeparamref name="TResponse"/>. Handled by exactly one query handler.</summary>
public interface IQuery<TResponse>;
