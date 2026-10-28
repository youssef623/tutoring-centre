using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance;

internal static class CardIssuance
{
    public static Result<IssuedCard> Issue(IStudentCardRepository cards, StudentCard? active, Guid centreId,
        Guid studentId, string studentName, Guid userId, DateTimeOffset now)
    {
        if (active is not null)
        {
            var revoked = active.Revoke(CardRevocationReason.Reissued, userId, now);
            if (revoked.IsFailure) { return Result<IssuedCard>.Failure(revoked.Error!); }
        }
        var token = CardToken.Generate();
        var issued = StudentCard.Issue(centreId, studentId, token, userId, now);
        if (issued.IsFailure) { return Result<IssuedCard>.Failure(issued.Error!); }
        cards.Add(issued.Value);
        return Result<IssuedCard>.Success(new(issued.Value.Id, studentId, studentName, token.Reveal(), now));
    }
}
