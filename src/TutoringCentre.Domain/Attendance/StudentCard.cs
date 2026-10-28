using System.Security.Cryptography;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Attendance;

public sealed class StudentCard : Entity, ITenantOwned
{
    private readonly byte[] tokenHash;

    private StudentCard(Guid centreId, Guid studentId, byte[] tokenHash, Guid issuedByUserId, DateTimeOffset at)
    {
        CentreId = centreId;
        StudentId = studentId;
        this.tokenHash = (byte[])tokenHash.Clone();
        IssuedByUserId = issuedByUserId;
        IssuedAt = at;
        Status = StudentCardStatus.Active;
    }

    public Guid CentreId { get; }
    public Guid StudentId { get; }
    public byte[] TokenHash => (byte[])tokenHash.Clone();
    public StudentCardStatus Status { get; private set; }
    public DateTimeOffset IssuedAt { get; }
    public Guid IssuedByUserId { get; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? RevokedByUserId { get; private set; }
    public CardRevocationReason? RevocationReason { get; private set; }

    public static Result<StudentCard> Issue(Guid centreId, Guid studentId, CardToken token, Guid issuedByUserId, DateTimeOffset at)
    {
        ArgumentNullException.ThrowIfNull(token);
        return centreId == Guid.Empty || studentId == Guid.Empty || issuedByUserId == Guid.Empty
            ? Result<StudentCard>.Failure(ReferenceRequired())
            : Result<StudentCard>.Success(new StudentCard(centreId, studentId, token.Hash(), issuedByUserId, at));
    }

    public Result Revoke(CardRevocationReason reason, Guid byUserId, DateTimeOffset at)
    {
        if (Status == StudentCardStatus.Revoked)
        {
            return Result.Failure(Error.Rule("card.already_revoked", "The card has already been revoked."));
        }

        if (byUserId == Guid.Empty)
        {
            return Result.Failure(ReferenceRequired());
        }

        if (!Enum.IsDefined(reason))
        {
            throw new ArgumentOutOfRangeException(nameof(reason));
        }

        Status = StudentCardStatus.Revoked;
        RevocationReason = reason;
        RevokedByUserId = byUserId;
        RevokedAt = at;
        return Result.Success();
    }

    public bool Matches(ReadOnlySpan<byte> hash) => CryptographicOperations.FixedTimeEquals(tokenHash, hash);

    private static Error ReferenceRequired() => Error.Validation("card.reference_required", "A non-empty reference is required.");
}
