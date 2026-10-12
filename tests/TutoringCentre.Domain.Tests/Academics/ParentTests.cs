using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class ParentTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly PhoneNumber Phone = PhoneNumber.Create("01012345678").Value;
    private static readonly PhoneNumber OtherPhone = PhoneNumber.Create("01112345678").Value;

    [Fact]
    public void Create_WithValidInput_Succeeds()
    {
        var result = Parent.Create(CentreId, "  Omar Hassan  ", Phone);

        Assert.True(result.IsSuccess);
        var parent = result.Value;
        Assert.Equal(CentreId, parent.CentreId);
        Assert.Equal("Omar Hassan", parent.FullName);
        Assert.Equal(Phone, parent.Phone);
        Assert.NotEqual(Guid.Empty, parent.Id);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired()
    {
        var result = Parent.Create(Guid.Empty, "Omar Hassan", Phone);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.centre_required", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    [Fact]
    public void Create_With1CharacterName_ReturnsNameInvalid() => AssertNameInvalid(Parent.Create(CentreId, "O", Phone));

    [Fact]
    public void Create_With121CharacterName_ReturnsNameInvalid() =>
        AssertNameInvalid(Parent.Create(CentreId, new string('a', 121), Phone));

    [Fact]
    public void Create_With2CharacterName_Succeeds()
    {
        var result = Parent.Create(CentreId, "Al", Phone);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_With120CharacterName_Succeeds()
    {
        var name = new string('a', 120);

        var result = Parent.Create(CentreId, name, Phone);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.FullName);
    }

    [Fact]
    public void Create_WithArabicName_IsKeptAsTypedWithNormalizedSearchName()
    {
        var result = Parent.Create(CentreId, "أحمد محمد", Phone);

        Assert.True(result.IsSuccess);
        var parent = result.Value;
        Assert.Equal("أحمد محمد", parent.FullName);
        Assert.Equal(SearchText.Normalize("أحمد محمد"), parent.SearchName);
        Assert.Equal(SearchText.Normalize("احمد محمد"), parent.SearchName);
    }

    [Fact]
    public void Update_WithValidInput_Succeeds()
    {
        var parent = Parent.Create(CentreId, "Omar Hassan", Phone).Value;

        var result = parent.Update("Omar Hassan Ali", OtherPhone);

        Assert.True(result.IsSuccess);
        Assert.Equal("Omar Hassan Ali", parent.FullName);
        Assert.Equal(OtherPhone, parent.Phone);
    }

    [Fact]
    public void Update_WithInvalidName_FailsAndLeavesNameAndPhoneUnchanged()
    {
        var parent = Parent.Create(CentreId, "Omar Hassan", Phone).Value;

        var result = parent.Update("", OtherPhone);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.name_invalid", result.Error!.Code);
        Assert.Equal("Omar Hassan", parent.FullName);
        Assert.Equal(Phone, parent.Phone);
    }

    private static void AssertNameInvalid(Result<Parent> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("parent.name_invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        Assert.Contains("fullName", result.Error.Fields!.Keys);
    }
}
