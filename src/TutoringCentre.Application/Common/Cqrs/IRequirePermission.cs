using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>
/// Marks a command or query that needs one permission to run. The dispatcher checks it after the
/// tenant step and before a transaction begins. A request without this marker is unaffected.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "IRequirePermission is the name specified by the Day 28 authorization contract.")]
public interface IRequirePermission
{
    /// <summary>The one permission key (from <see cref="TutoringCentre.Domain.Identity.Permissions"/>) this request needs.</summary>
    string RequiredPermission { get; }
}
