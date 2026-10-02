namespace TutoringCentre.Application.Centres.Commands.CreateCentre;

/// <summary>What the caller needs to know about the created centre.</summary>
public sealed record CreateCentreResult(Guid CentreId, string Slug);
