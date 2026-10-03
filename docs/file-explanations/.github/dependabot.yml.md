# .github/dependabot.yml

## Purpose

Tells GitHub Dependabot to open weekly pull requests updating NuGet, npm and GitHub Actions dependencies.

## Where It Fits

Read by GitHub. Targets `/` (NuGet - edits `Directory.Packages.props`), `/frontend` (npm - edits `package.json`/`package-lock.json`) and `/` for `github-actions` (edits workflow files).

## Walkthrough

Three `updates` entries, each `schedule.interval: weekly`, each with a group (`nuget-minor-and-patch`, `npm-minor-and-patch`, `actions-minor-and-patch`) containing all packages (`patterns: ["*"]`) limited to `update-types: [minor, patch]`. Effect: minor and patch bumps arrive as one PR per ecosystem; major bumps arrive as individual PRs. Because CI runs on those PRs, the CI is the safety net for upgrades.

## Concepts Used

### Continuous Integration

#### What it means

CI runs automated build, test and analysis on every change in a clean machine so that regressions are caught before merge. Workflows are YAML files describing jobs (parallel units) made of steps.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#11-configuration--environment--deployment](../../PROJECT_OVERVIEW.md#11-configuration--environment--deployment).)

#### Where it appears in this file

Dependabot grouping.

#### How it works here

Dependabot opens PRs which trigger `ci.yml`.

#### Why it matters here

Keeps dependencies current with little noise.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None.

## Gotchas and Issues

Version-pinned tools outside these ecosystems (the `dotnet-ef` tool in `.config/dotnet-tools.json`, `global.json` SDK) are not covered by these entries.

## Related Files

- [`.github/workflows/ci.yml`](workflows/ci.yml.md)
- [`Directory.Packages.props`](../Directory.Packages.props.md)
- [`frontend/package.json`](../frontend/package.json.md)
