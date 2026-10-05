# .github/workflows/secret-scan.yml

## Purpose

Scans the full git history for committed secrets with gitleaks.

## Where It Fits

GitHub Actions workflow on pull requests and pushes to `main`. Complements the repo's secret hygiene (empty `ConnectionStrings:Postgres`, git-ignored `.env`).

## Walkthrough

`permissions`: workflow `contents: read`; job adds `pull-requests: write` so the action can comment on a PR when it finds a leak. Steps: `actions/checkout@v6` with `fetch-depth: 0` (full history - the default shallow clone would hide old commits); `gitleaks/gitleaks-action@v3` with `GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}`. No `.gitleaks.toml` exists, so gitleaks default rules apply.

## Concepts Used

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#11-configuration--environment--deployment](../../../PROJECT_OVERVIEW2.md#11-configuration--environment--deployment).)

#### Where it appears in this file

The gitleaks step.

#### How it works here

Scans every commit in history against pattern rules.

#### Why it matters here

Backstop for the convention that secrets live only in user-secrets/env vars.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Uses the built-in `secrets.GITHUB_TOKEN` (provided by GitHub, not defined in the repo).

## Gotchas and Issues

Gitleaks only detects patterns; a weak but pattern-less password would pass.

## Related Files

- [`.github/workflows/ci.yml`](ci.yml.md)
- [`.gitignore`](../../.gitignore.md)
- [`.env.example`](../../.env.example.md)
