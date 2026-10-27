# `src/TutoringCentre.Api/TutoringCentre.Api.csproj`

## Purpose

Current configuration file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **configuration**. [Open current file](../../../../src/TutoringCentre.Api/TutoringCentre.Api.csproj). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 30 lines and 1,574 bytes. Source hash: `15f0b5fa2e959fd23116ba2cf0f276739d102b9340c30114d575041c57494fae`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 8: `<ProjectReference Include="..\TutoringCentre.Application\TutoringCentre.Application.csproj" />`
- Line 9: `<ProjectReference Include="..\TutoringCentre.Infrastructure\TutoringCentre.Infrastructure.csproj" />`
- Line 15: `<PackageReference Include="Microsoft.AspNetCore.OpenApi" />`
- Line 16: `<PackageReference Include="Microsoft.EntityFrameworkCore.Design">`
- Line 20: `<PackageReference Include="Microsoft.Extensions.ApiDescription.Server">`
- Line 24: `<PackageReference Include="Npgsql.OpenTelemetry" />`
- Line 25: `<PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" />`
- Line 26: `<PackageReference Include="OpenTelemetry.Extensions.Hosting" />`
- Line 27: `<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" />`
- Line 28: `<PackageReference Include="Serilog.AspNetCore" />`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <UserSecretsId>a0748b74-413e-4737-b25c-2a2e46b2d4e6</UserSecretsId>
    <OpenApiGenerateDocuments>true</OpenApiGenerateDocuments>
    <OpenApiDocumentsDirectory>$(MSBuildProjectDirectory)/../../frontend/openapi</OpenApiDocumentsDirectory>
  </PropertyGroup>
    <ItemGroup>
    <ProjectReference Include="..\TutoringCentre.Application\TutoringCentre.Application.csproj" />
    <ProjectReference Include="..\TutoringCentre.Infrastructure\TutoringCentre.Infrastructure.csproj" />
  </ItemGroup>
  <ItemGroup>
    <InternalsVisibleTo Include="TutoringCentre.Architecture.Tests" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.ApiDescription.Server">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Npgsql.OpenTelemetry" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" />
    <PackageReference Include="Serilog.AspNetCore" />
  </ItemGroup>
</Project>
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
