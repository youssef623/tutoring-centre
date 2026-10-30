using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Domain.Tests.Conversations;

public sealed class WebChannelAccountTests
{
    [Fact]
    public void RegisterWeb_UsesCentreSlugAndNeedsNoPhone()
    {
        var centre = Guid.NewGuid();
        var result = ChannelAccount.RegisterWeb(centre, "web:nile-centre");
        Assert.True(result.IsSuccess);
        var account = result.Value;
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(centre, account.CentreId);
        Assert.Equal(ChannelProvider.Web, account.Provider);
        Assert.Equal("web:nile-centre", account.ExternalAccountId);
        Assert.Null(account.DisplayPhone);
        Assert.Equal("active", account.Status);
        account.Disable();
        Assert.Equal("disabled", account.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nile-centre")]
    [InlineData("WEB:nile-centre")]
    [InlineData("web:")]
    [InlineData("web:-nile")]
    [InlineData("web:nile-")]
    [InlineData("web:nile--centre")]
    [InlineData("web:Nile")]
    [InlineData("web:nile centre")]
    [InlineData("web:nile/centre")]
    [InlineData("web:nile\n")]
    public void RegisterWeb_RejectsMalformedExternalId(string? externalId)
    {
        var result = ChannelAccount.RegisterWeb(Guid.NewGuid(), externalId!);
        Assert.True(result.IsFailure);
        Assert.Equal("conversations.message_invalid", result.Error?.Code);
    }

    [Fact]
    public void RegisterWeb_RejectsEmptyCentreAndExcessiveExternalId()
    {
        Assert.True(ChannelAccount.RegisterWeb(Guid.Empty, "web:nile-centre").IsFailure);
        Assert.True(ChannelAccount.RegisterWeb(Guid.NewGuid(), "web:" + new string('a', 61)).IsFailure);
        Assert.True(ChannelAccount.RegisterWeb(Guid.NewGuid(), "web:" + new string('a', 60)).IsSuccess);
    }

    [Fact]
    public void LegacyWhatsappFactory_KeepsNumericIdAndPhoneAndDoesNotAcceptWebProvider()
    {
        var centre = Guid.NewGuid();
        var phone = PhoneNumber.Create("+201000000091").Value;
        var result = ChannelAccount.Register(centre, ChannelProvider.Whatsapp, "100000000000001", phone);
        Assert.True(result.IsSuccess);
        Assert.Equal(ChannelProvider.Whatsapp, result.Value.Provider);
        Assert.Equal("100000000000001", result.Value.ExternalAccountId);
        Assert.Equal(phone, result.Value.DisplayPhone);
        Assert.True(ChannelAccount.Register(centre, ChannelProvider.Whatsapp, "web:nile-centre", phone).IsFailure);
        Assert.True(ChannelAccount.Register(centre, ChannelProvider.Web, "100000000000001", phone).IsFailure);
        Assert.Throws<ArgumentNullException>(() => ChannelAccount.Register(centre, ChannelProvider.Whatsapp, "100000000000001", null!));
    }
}
