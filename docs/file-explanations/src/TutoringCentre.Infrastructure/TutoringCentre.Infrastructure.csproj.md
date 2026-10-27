# `src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj`

## Purpose

Current configuration file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **configuration**. [Open current file](../../../../src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 1,070 bytes. Source hash: `30f9bbbfe82700f56ec632cc461fcc0f242ef7579927cae283cbd0549521ffa7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 3: `<ProjectReference Include="..\TutoringCentre.Application\TutoringCentre.Application.csproj" />`
- Line 4: `<ProjectReference Include="..\TutoringCentre.Domain\TutoringCentre.Domain.csproj" />`
- Line 7: `<PackageReference Include="AspNetCore.HealthChecks.NpgSql" />`
- Line 8: `<PackageReference Include="EFCore.NamingConventions" />`
- Line 9: `<PackageReference Include="Microsoft.AspNetCore.DataProtection.EntityFrameworkCore" />`
- Line 10: `<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />`
- Line 11: `<PackageReference Include="Microsoft.EntityFrameworkCore" />`
- Line 12: `<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" />`
- Line 13: `<PackageReference Include="Microsoft.Extensions.Options.DataAnnotations" />`
- Line 14: `<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\TutoringCentre.Application\TutoringCentre.Application.csproj" />
    <ProjectReference Include="..\TutoringCentre.Domain\TutoringCentre.Domain.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="AspNetCore.HealthChecks.NpgSql" />
    <PackageReference Include="EFCore.NamingConventions" />
    <PackageReference Include="Microsoft.AspNetCore.DataProtection.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.Options.DataAnnotations" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
  </ItemGroup>
  <ItemGroup>
    <InternalsVisibleTo Include="TutoringCentre.Architecture.Tests" />
    <InternalsVisibleTo Include="TutoringCentre.Infrastructure.Tests" />
  </ItemGroup>
</Project>
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
