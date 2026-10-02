namespace TutoringCentre.Application.Common.Security;

/// <summary>Read-only access to the actor of the current scope (HTTP request or job).</summary>
public interface ICurrentActor
{
    Actor Actor { get; }
}
