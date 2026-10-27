# `.github/workflows/ci.yml`

## Purpose

Current CI/CD file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **CI/CD**. [Open current file](../../../../.github/workflows/ci.yml). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 346 lines and 13,260 bytes. Source hash: `f9a3a693c83b11e5a3923c7a2f459040c7a51dabf5ebb8e54114a47be5d35994`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 8: A newer push to the same branch/PR cancels the run that is now out of date.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```yaml
name: CI

on:
  pull_request:
  push:
    branches: [main]

# A newer push to the same branch/PR cancels the run that is now out of date.
concurrency:
  group: ci-${{ github.ref }}
  cancel-in-progress: true

permissions:
  contents: read

jobs:
  backend:
    name: backend
    runs-on: ubuntu-latest
    steps:
      - name: Check out
        uses: actions/checkout@v6

      - name: Set up .NET from global.json
        uses: actions/setup-dotnet@v5
        with:
          global-json-file: global.json

      - name: Restore
        run: dotnet restore TutoringCentre.slnx

      - name: Build
        run: dotnet build TutoringCentre.slnx --no-restore -c Release

      - name: OpenAPI document is up to date
        run: git diff --exit-code -- frontend/openapi

      - name: Tool restore
        run: dotnet tool restore

      - name: Check for model changes without a migration
        # Design-time model comparison needs valid-looking connection strings but never connects.
        # The design-time factory (Api/Cli) reads the migrations one; the runtime one is unused here but still
        # required by startup validation.
        env:
          ConnectionStrings__Postgres: Host=localhost;Database=ci;Username=ci;Password=ci
          ConnectionStrings__PostgresMigrations: Host=localhost;Database=ci;Username=ci;Password=ci
        run: >
          dotnet ef migrations has-pending-model-changes
          --project src/TutoringCentre.Infrastructure
          --startup-project src/TutoringCentre.Api
          --no-build --configuration Release

      - name: Test (all test projects, including architecture tests)
        run: dotnet test TutoringCentre.slnx --no-build -c Release

  frontend:
    name: frontend
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: frontend
    steps:
      - name: Check out
        uses: actions/checkout@v6

      - name: Set up Node.js (LTS) with npm cache
        uses: actions/setup-node@v6
        with:
          node-version: lts/*
          cache: npm
          cache-dependency-path: frontend/package-lock.json

      - name: Install exactly the lock file
        run: npm ci

      - name: Generated API client is up to date
        working-directory: frontend
        run: |
          npm run generate:api
          git diff --exit-code -- src/api/generated

      - name: Lint
        run: npm run lint

      - name: Type-check
        run: npm run typecheck

      - name: Translation keys match (en/ar)
        run: npm run i18n:check
```

The excerpt stops at line 90; use the original file for the remaining 256 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
