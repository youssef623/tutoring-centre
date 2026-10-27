# `src/TutoringCentre.Application/TutoringCentre.Application.csproj`

## Purpose

Current configuration file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **configuration**. [Open current file](../../../../src/TutoringCentre.Application/TutoringCentre.Application.csproj). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 665 bytes. Source hash: `ba8a031af8f158d0f7207176231328ab4c0405153ae9afc5bf6d2d536ae6de2d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 3: `<ProjectReference Include="..\TutoringCentre.Domain\TutoringCentre.Domain.csproj" />`
- Line 6: `<PackageReference Include="FluentValidation" />`
- Line 7: `<PackageReference Include="FluentValidation.DependencyInjectionExtensions" />`
- Line 8: `<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />`
- Line 9: `<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\TutoringCentre.Domain\TutoringCentre.Domain.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="FluentValidation" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
  </ItemGroup>
  <ItemGroup>
    <InternalsVisibleTo Include="TutoringCentre.Architecture.Tests" />
    <InternalsVisibleTo Include="TutoringCentre.Application.Tests" />
  </ItemGroup>
</Project>
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
