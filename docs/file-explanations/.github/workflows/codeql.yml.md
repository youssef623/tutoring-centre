# `.github/workflows/codeql.yml`

## Purpose

Current CI/CD file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **CI/CD**. [Open current file](../../../../.github/workflows/codeql.yml). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 52 lines and 1,271 bytes. Source hash: `ba574a6a6987bdcf17f294416ddc821c7a865646cb5ff7bf724169fab19c5853`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```yaml
name: CodeQL

on:
  pull_request:
  push:
    branches: [main]
  schedule:
    - cron: "27 3 * * 1" # weekly, Monday 03:27 UTC

permissions:
  contents: read

jobs:
  analyze:
    name: Analyze (${{ matrix.language }})
    runs-on: ubuntu-latest
    permissions:
      contents: read
      actions: read
      security-events: write # upload results to the Security tab
    strategy:
      fail-fast: false
      matrix:
        include:
          - language: csharp
            build-mode: manual
          - language: javascript-typescript
            build-mode: none
    steps:
      - name: Check out
        uses: actions/checkout@v6

      - name: Set up .NET from global.json
        if: matrix.language == 'csharp'
        uses: actions/setup-dotnet@v5
        with:
          global-json-file: global.json

      - name: Initialize CodeQL
        uses: github/codeql-action/init@v4
        with:
          languages: ${{ matrix.language }}
          build-mode: ${{ matrix.build-mode }}

      - name: Build C# for analysis
        if: matrix.build-mode == 'manual'
        run: dotnet build TutoringCentre.slnx -c Release

      - name: Analyze
        uses: github/codeql-action/analyze@v4
        with:
          category: "/language:${{ matrix.language }}"
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
