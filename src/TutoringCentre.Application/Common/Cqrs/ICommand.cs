namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>
/// An intention to change state, producing <typeparamref name="TResponse"/> on success
/// (use <see cref="Unit"/> when there is nothing to return). Handled by exactly one command handler.
/// </summary>
public interface ICommand<TResponse>;
