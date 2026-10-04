using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Verifies staff credentials, without saying how. Every failure is <c>Unauthenticated("auth.invalid_credentials")</c>
/// with the same message: an unknown email, a wrong password and a locked account are indistinguishable to the
/// caller, so nothing here can be used to enumerate accounts. Implemented in Infrastructure.
/// </summary>
public interface IAuthenticationService
{
    Task<Result<AuthenticatedUser>> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken);
}

/// <summary>The only facts a successful verification yields: who, and the stamp to compare on later requests.</summary>
public sealed record AuthenticatedUser(Guid UserId, string SecurityStamp);
