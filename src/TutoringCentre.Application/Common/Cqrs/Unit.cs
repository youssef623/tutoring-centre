namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>The "no data" response for commands that only succeed or fail, e.g. ICommand&lt;Unit&gt;.</summary>
public readonly record struct Unit
{
    /// <summary>The single value of <see cref="Unit"/>. A static readonly field is default-initialised.</summary>
    public static readonly Unit Value;
}
