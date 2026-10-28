namespace TutoringCentre.Application.Attendance;

public sealed record IssuedCard(Guid CardId, Guid StudentId, string StudentName, string Token, DateTimeOffset IssuedAt)
{
    // Issue results contain bearer credentials. Accidental interpolation must never reveal them.
    public override string ToString() => $"IssuedCard {{ CardId = {CardId}, StudentId = {StudentId} }}";
}

public sealed record IssuedGroupCards(Guid GroupId, string GroupName, IReadOnlyList<IssuedCard> Cards);

public sealed record StudentCardDto(Guid CardId, string Status, DateTimeOffset IssuedAt, string IssuedByName,
    string? RevocationReason, DateTimeOffset? RevokedAt, uint Version);

public interface IStudentCardReadService
{
    Task<StudentCardDto?> GetMostRecentAsync(Guid studentId, CancellationToken ct);
}
