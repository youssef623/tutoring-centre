using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Attendance;

public sealed class StudentCardTests
{
    private static readonly Guid Centre = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Student = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid Issuer = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly DateTimeOffset At = new(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);
    private static readonly CardToken Token = CardToken.Parse("TCM1." + new string('A', 43)).Value;
    private static StudentCard Issue() => StudentCard.Issue(Centre, Student, Token, Issuer, At).Value;

    [Fact]
    public void Issue_SetsFieldsAndDefensivelyCopiesHash()
    {
        var card = Issue();
        Assert.NotEqual(Guid.Empty, card.Id);
        Assert.IsAssignableFrom<ITenantOwned>(card);
        Assert.Equal(Centre, card.CentreId);
        Assert.Equal(Student, card.StudentId);
        Assert.Equal(Issuer, card.IssuedByUserId);
        Assert.Equal(At, card.IssuedAt);
        Assert.Equal(StudentCardStatus.Active, card.Status);
        Assert.Null(card.RevokedAt);
        Assert.Null(card.RevokedByUserId);
        Assert.Null(card.RevocationReason);
        var hash = card.TokenHash;
        Assert.Equal(32, hash.Length);
        Assert.Equal(Token.Hash(), hash);
        hash[0] ^= 255;
        Assert.Equal(Token.Hash(), card.TokenHash);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Issue_EmptyReference_Fails(int empty)
    {
        var result = StudentCard.Issue(empty == 0 ? Guid.Empty : Centre, empty == 1 ? Guid.Empty : Student,
            Token, empty == 2 ? Guid.Empty : Issuer, At);
        Assert.Equal("card.reference_required", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    [Theory]
    [InlineData(CardRevocationReason.Reissued)]
    [InlineData(CardRevocationReason.Lost)]
    [InlineData(CardRevocationReason.Damaged)]
    [InlineData(CardRevocationReason.Withdrawn)]
    public void Revoke_Once_SetsMetadataAndSecondAttemptChangesNothing(CardRevocationReason reason)
    {
        var card = Issue();
        Assert.True(card.Revoke(reason, Issuer, At.AddHours(1)).IsSuccess);
        var again = card.Revoke(CardRevocationReason.Reissued, Student, At.AddDays(1));
        Assert.Equal("card.already_revoked", again.Error!.Code);
        Assert.Equal(ErrorKind.Rule, again.Error.Kind);
        Assert.Equal(StudentCardStatus.Revoked, card.Status);
        Assert.Equal(reason, card.RevocationReason);
        Assert.Equal(Issuer, card.RevokedByUserId);
        Assert.Equal(At.AddHours(1), card.RevokedAt);
        Assert.Equal(At, card.IssuedAt);
        Assert.Equal(Issuer, card.IssuedByUserId);
        Assert.Equal(Centre, card.CentreId);
        Assert.Equal(Student, card.StudentId);
        Assert.Equal(Token.Hash(), card.TokenHash);
    }

    [Fact]
    public void Matches_AcceptsOwnHashAndRejectsOthers()
    {
        var card = Issue();
        Assert.True(card.Matches(Token.Hash()));
        Assert.False(card.Matches(CardToken.Parse("TCM1." + new string('B', 43)).Value.Hash()));
        Assert.False(card.Matches(new byte[31]));
        Assert.False(card.Matches([]));
    }

    [Fact]
    public void Revoke_InvalidInputs_DoNotChangeState()
    {
        var card = Issue();
        Assert.Equal("card.reference_required", card.Revoke(CardRevocationReason.Lost, Guid.Empty, At).Error!.Code);
        Assert.Throws<ArgumentOutOfRangeException>(() => card.Revoke((CardRevocationReason)99, Issuer, At));
        Assert.Equal(StudentCardStatus.Active, card.Status);
        Assert.Null(card.RevokedAt);
        Assert.Null(card.RevokedByUserId);
        Assert.Null(card.RevocationReason);
    }
}
