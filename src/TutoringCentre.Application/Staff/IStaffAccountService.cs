using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Staff;

/// <summary>
/// Ensures a login account exists for an email, without the use case knowing which framework backs it
/// (ASP.NET Identity, Infrastructure-only). Participates in the dispatcher's transaction: a command that fails
/// after calling this leaves no account behind, the same as any other tracked write.
/// </summary>
public interface IStaffAccountService
{
    /// <summary>
    /// An existing account (normalized email match) returns its ID, <c>Created: false</c>, no password, and
    /// its profile untouched. A new email creates the account with a cryptographically random temporary
    /// password that satisfies the password policy and the must-change-password flag set, returning that
    /// password in plain text — present only in this result, never logged, never stored elsewhere.
    /// </summary>
    Task<Result<EnsureStaffAccountResult>> EnsureAccountAsync(
        string email, string displayName, string preferredLocale, CancellationToken ct);
}

public sealed record EnsureStaffAccountResult(Guid UserId, bool Created, string? TemporaryPassword);
