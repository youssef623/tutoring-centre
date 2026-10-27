namespace TutoringCentre.Application.Academics.Contracts;

/// <summary>Publishes the current actor's teaching ownership without exposing its implementation.</summary>
public interface ITeachingScope
{
    Task<TeachingScope> GetAsync(CancellationToken ct);
}
