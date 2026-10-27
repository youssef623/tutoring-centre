# `tests/TutoringCentre.Infrastructure.Tests/TutoringCentre.Infrastructure.Tests.csproj`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Infrastructure.Tests/TutoringCentre.Infrastructure.Tests.csproj). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,286 bytes. Source hash: `9033dda0e76efde37acde916a36da0c8baa89ecdc29a54667de13885eea41fcb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 12: `<PackageReference Include="coverlet.collector">`
- Line 16: `<PackageReference Include="Microsoft.Extensions.Configuration" />`
- Line 17: `<PackageReference Include="Microsoft.Extensions.DependencyInjection" />`
- Line 18: `<PackageReference Include="Microsoft.Extensions.Logging" />`
- Line 19: `<PackageReference Include="Microsoft.Extensions.TimeProvider.Testing" />`
- Line 20: `<PackageReference Include="Microsoft.NET.Test.Sdk" />`
- Line 21: `<PackageReference Include="Respawn" />`
- Line 22: `<PackageReference Include="Testcontainers.PostgreSql" />`
- Line 23: `<PackageReference Include="xunit" />`
- Line 24: `<PackageReference Include="xunit.runner.visualstudio">`
- Line 31: `<ProjectReference Include="..\..\src\TutoringCentre.Infrastructure\TutoringCentre.Infrastructure.csproj" />`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.Configuration" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" />
    <PackageReference Include="Microsoft.Extensions.Logging" />
    <PackageReference Include="Microsoft.Extensions.TimeProvider.Testing" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="Respawn" />
    <PackageReference Include="Testcontainers.PostgreSql" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\TutoringCentre.Infrastructure\TutoringCentre.Infrastructure.csproj" />
  </ItemGroup>

</Project>
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
