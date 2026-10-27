# `tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs`

## Purpose

Declares `ProxyOptionsValidatorTests`, `FakeHostEnvironment`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 60 lines and 2,013 bytes. Source hash: `3cd5083690c2404734d988f9d22b652f5f74623eec35e860aa0db11a4a2d422b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.FileProviders;`
- Line 2: `using Microsoft.Extensions.Hosting;`
- Line 3: `using TutoringCentre.Api.Http;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L7): `public sealed class ProxyOptionsValidatorTests`
- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L10): `public void Production_WithNoKnownNetworks_Fails()`
- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L20): `public void Production_WithAKnownNetwork_Succeeds()`
- [Line 32](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L32): `public void OutsideProduction_WithNoKnownNetworks_Succeeds(string environmentName)`
- [Line 42](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L42): `public void AnyEnvironment_WithAMalformedNetwork_Fails()`
- [Line 51](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L51): `private static FakeHostEnvironment FakeEnvironment(string environmentName) => new(environmentName);`
- [Line 53](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L53): `private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
