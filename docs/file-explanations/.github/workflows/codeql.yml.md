# .github/workflows/codeql.yml

## Purpose
Runs GitHub **CodeQL** static security analysis for C# and JavaScript/TypeScript, and uploads the results to the repository's Security tab.

## Where it fits
CI/CD security gate (`README.md:98`). It builds `TutoringCentre.slnx` for the C# analysis.

## Walkthrough
- **Lines 3–8, triggers:** PRs, pushes to `main`, and a weekly cron `27 3 * * 1` (Monday 03:27 UTC).
- **Lines 10–11:** workflow-level `contents: read`.
- **Lines 17–20:** the job adds `actions: read` and `security-events: write` (needed to upload SARIF).
- **Lines 21–28, matrix:** `csharp` with `build-mode: manual`, and `javascript-typescript` with `build-mode: none`. `fail-fast: false` lets one language finish even if the other fails.
- **Steps:**
  1. checkout.
  2. setup-dotnet (C# only).
  3. `codeql-action/init@v4`.
  4. `dotnet build TutoringCentre.slnx -c Release` (manual mode only).
  5. `codeql-action/analyze@v4` with category `/language:<lang>`.

## Concepts used
- **Static application security testing (SAST).**
- **Matrix builds.**
- **CodeQL build modes:** `manual` means you run the build yourself; `none` analyses source without building.

## Data and control flow
Source → CodeQL database → queries → SARIF → Security tab.

## Configuration and environment
None beyond the job permissions.

## Gotchas and issues
None found.

## Related files
- [ci.yml](ci.yml.md)
- [secret-scan.yml](secret-scan.yml.md)
