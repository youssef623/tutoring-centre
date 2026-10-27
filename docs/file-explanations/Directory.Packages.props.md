# `Directory.Packages.props`

## Purpose

Current configuration file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **configuration**. [Open current file](../../Directory.Packages.props). [Complete file index](INDEX.md). [Current teaching overview](../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 2,931 bytes. Source hash: `c9c83f2d23300532e829e35e1b331a24ea349fc96ab9f5a264086d99d0cf283d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- PackageVersion entries go here as projects add dependencies. -->
  </ItemGroup>
  <ItemGroup>
    <PackageVersion Include="AspNetCore.HealthChecks.NpgSql" Version="9.0.0" />
    <PackageVersion Include="coverlet.collector" Version="10.1.0" />
    <PackageVersion Include="EFCore.NamingConventions" Version="10.0.1" />
    <PackageVersion Include="FluentValidation" Version="12.1.1" />
    <PackageVersion Include="FluentValidation.DependencyInjectionExtensions" Version="12.1.1" />
    <PackageVersion Include="Microsoft.AspNetCore.DataProtection.EntityFrameworkCore" Version="10.0.13" />
    <PackageVersion Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.13" />
    <PackageVersion Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.13" />
    <PackageVersion Include="Microsoft.AspNetCore.OpenApi" Version="10.0.13" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.13" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.ApiDescription.Server" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.Configuration" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.Configuration.Abstractions" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.Logging" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.Options.DataAnnotations" Version="10.0.13" />
    <PackageVersion Include="Microsoft.Extensions.TimeProvider.Testing" Version="10.10.0" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="18.10.1" />
    <PackageVersion Include="NetArchTest.Rules" Version="1.3.2" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.3" />
    <PackageVersion Include="Npgsql.OpenTelemetry" Version="10.0.1" />
    <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.15.3" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.15.3" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.16.0" />
    <PackageVersion Include="Respawn" Version="7.0.0" />
    <PackageVersion Include="Serilog.AspNetCore" Version="10.0.0" />
    <PackageVersion Include="Testcontainers.PostgreSql" Version="4.15.0" />
    <PackageVersion Include="xunit" Version="2.9.3" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="4.0.0" />
  </ItemGroup>
</Project>
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
