# .github/workflows/codeql.yml

## Purpose

Runs GitHub's CodeQL static security analysis on the C# and TypeScript code.

## Where It Fits

GitHub Actions workflow independent of `ci.yml`. Results go to the repository's Security tab (`security-events: write`).

## Walkthrough

Triggers: pull requests, pushes to `main`, weekly cron `27 3 * * 1` (Monday 03:27 UTC). Workflow-level `permissions: contents: read`; job-level adds `actions: read` and `security-events: write`.
Matrix (`fail-fast: false`): `csharp` with `build-mode: manual`, `javascript-typescript` with `build-mode: none`.
Steps: checkout; (C# only) `setup-dotnet` from `global.json`; `github/codeql-action/init@v4` with the language/build-mode; (manual only) `dotnet build TutoringCentre.slnx -c Release` so CodeQL can observe the compilation; `github/codeql-action/analyze@v4` with `category: /language:<lang>`.
`fail-fast: false` means a failure in one language does not cancel the other.

## Concepts Used

### Continuous Integration

#### What it means

CI runs automated build, test and analysis on every change in a clean machine so that regressions are caught before merge. Workflows are YAML files describing jobs (parallel units) made of steps.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#11-configuration--environment--deployment](../../../PROJECT_OVERVIEW.md#11-configuration--environment--deployment).)

#### Where it appears in this file

Matrix job `analyze`.

#### How it works here

One job instance per language; C# must be compiled under CodeQL's tracer, JavaScript/TypeScript is analysed from source.

#### Why it matters here

Security scanning without any code in the repo.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

No variables or secrets (uses the automatic token). Cron `27 3 * * 1`.

## Gotchas and Issues

Weekly schedule means findings can appear without a code change.

## Related Files

- [`.github/workflows/ci.yml`](ci.yml.md)
- [`.github/workflows/secret-scan.yml`](secret-scan.yml.md)
- [`global.json`](../../global.json.md)
