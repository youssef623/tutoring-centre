namespace TutoringCentre.Application.Centres.Contracts;

/// <summary>Lists identifiers for trusted background fan-out. Tenant data still requires a tenant scope.</summary>
public interface ICentreDirectory
{
    Task<IReadOnlyList<Guid>> ListCentreIdsAsync(CancellationToken cancellationToken);
}
