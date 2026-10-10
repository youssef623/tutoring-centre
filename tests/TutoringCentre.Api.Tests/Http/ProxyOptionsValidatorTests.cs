using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using TutoringCentre.Api.Http;

namespace TutoringCentre.Api.Tests.Http;

public sealed class ProxyOptionsValidatorTests
{
    [Fact]
    public void Production_WithNoKnownNetworks_Fails()
    {
        var validator = new ProxyOptionsValidator(FakeEnvironment("Production"));

        var result = validator.Validate(null, new ProxyOptions { KnownNetworks = [] });

        Assert.True(result.Failed);
    }

    [Fact]
    public void Production_WithAKnownNetwork_Succeeds()
    {
        var validator = new ProxyOptionsValidator(FakeEnvironment("Production"));

        var result = validator.Validate(null, new ProxyOptions { KnownNetworks = ["10.0.0.0/8"] });

        Assert.False(result.Failed);
    }

    [Theory]
    [InlineData("Development")]
    [InlineData("Testing")]
    public void OutsideProduction_WithNoKnownNetworks_Succeeds(string environmentName)
    {
        var validator = new ProxyOptionsValidator(FakeEnvironment(environmentName));

        var result = validator.Validate(null, new ProxyOptions { KnownNetworks = [] });

        Assert.False(result.Failed);
    }

    [Fact]
    public void AnyEnvironment_WithAMalformedNetwork_Fails()
    {
        var validator = new ProxyOptionsValidator(FakeEnvironment("Development"));

        var result = validator.Validate(null, new ProxyOptions { KnownNetworks = ["not-a-cidr"] });

        Assert.True(result.Failed);
    }

    private static FakeHostEnvironment FakeEnvironment(string environmentName) => new(environmentName);

    private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "TutoringCentre.Api.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
